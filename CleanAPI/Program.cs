using Clean.Core.Llm;
using Clean.Core.Repositories;
using Clean.Core.Services;
using Clean.Data;
using Clean.Data.Repositories;
using Clean.Service;
using CleanAPI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});

builder.Services.AddProblemDetails();   // lets ASP.NET write a JSON body for errors that have none (401/403/404)
builder.Services.AddSingleton(TimeProvider.System);   // the real clock; ShabatMiddleware asks it for the time (tests pass a fake one)

builder.Services.AddEndpointsApiExplorer();

// 2. הגדרת Swagger עם אפשרות להזנת Token
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});

// 3. הגדרת בסיס הנתונים (Database)
// Like the JWT key, the connection string is a secret: User Secrets (dev) or an environment variable (production).
var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "ConnectionStrings:Default is not configured. Run: dotnet user-secrets set \"ConnectionStrings:Default\" \"<connection string>\" --project CleanAPI");
}
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));

// 4. הגדרת הזרקת תלויות (Dependency Injection) לשירותים של אפליקציית הניקיון
builder.Services.AddScoped<ICleanerRepository, CleanerRepository>();
builder.Services.AddScoped<ICleanerService, CleanerService>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IAvailabilityRepository, AvailabilityRepository>();
builder.Services.AddScoped<IAvailabilityService, AvailabilityService>();
builder.Services.AddScoped<IBlockedDateRepository, BlockedDateRepository>();
builder.Services.AddScoped<IBlockedDateService, BlockedDateService>();
builder.Services.AddScoped<ICleanerSearchService, CleanerSearchService>();
builder.Services.AddScoped<ISearchIntentService, SearchIntentService>();
builder.Services.AddScoped<ISearchConversationService, SearchConversationService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// The JWT key is a secret: it lives in User Secrets (dev) or an environment variable (production), never in Git.
// Fail fast with a clear message instead of crashing later with a NullReferenceException.
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "JWT:Key is not configured. Run: dotnet user-secrets set \"JWT:Key\" \"<long random string>\" --project CleanAPI");
}

// 5. הגדרת אימות (Authentication) - זה התיקון לשגיאת ה-500 שקיבלת
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// 6. הגדרת AutoMapper לסריקה של כל הפרופילים (גם ב-Core וגם ב-API)
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddHttpClient<ILocationService, LocationService>(client =>
{
    client.BaseAddress = new Uri("https://nominatim.openstreetmap.org/");
    client.DefaultRequestHeaders.UserAgent.ParseAdd("CleanMatch/1.0");
    client.Timeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddSingleton(new GeminiSettings(
    builder.Configuration["Gemini:ApiKey"] ?? "",
    builder.Configuration["Gemini:Model"] ?? "gemini-3.1-flash-lite",
    TimeSpan.FromSeconds(2)));
builder.Services.AddHttpClient<ILlmClient, GeminiClient>(client =>
{
    client.BaseAddress = new Uri("https://generativelanguage.googleapis.com/v1beta/");
    client.Timeout = TimeSpan.FromSeconds(30);
});
var policy = "policy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: policy, policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});


var app = builder.Build();

// 7. הגדרת ה-Middleware בסדר הנכון
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// חשוב: קודם אימות (מי את) ואז הרשאה (מה מותר לך)
app.UseCors(policy);
app.UseErrorHandling();     // catches exceptions from everything below it; placed after CORS so error answers keep the CORS headers
app.UseStatusCodePages();   // a bare 401/403/404 gets a JSON body too
app.UseAuthentication();
app.UseAuthorization();

app.UseShabbat();

app.MapControllers();

app.Run();