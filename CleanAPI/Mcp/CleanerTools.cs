using AutoMapper;
using Clean.Core.DTO;
using Clean.Core.Services;
using Microsoft.AspNetCore.Authorization;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Globalization;
using System.Security.Claims;
using System.Text.Json;

namespace CleanAPI.Mcp
{
    // The tools the agent may ask us to run. Like a Controller, this class stays thin: it checks who is asking,
    // turns the model's text arguments into real values, and calls the existing Services.
    [McpServerToolType]
    public class CleanerTools
    {
        private const int MaxResults = 5;
        // Relaxed escaping keeps Hebrew readable (and shorter, which saves tokens) instead of ב sequences.
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        private readonly ICleanerSearchService _search;
        private readonly ICustomerService _customers;
        private readonly IMapper _mapper;

        public CleanerTools(ICleanerSearchService search, ICustomerService customers, IMapper mapper)
        {
            _search = search;
            _customers = customers;
            _mapper = mapper;
        }

        // `user` is filled in by the SDK from the caller's token and is not part of the schema the model sees,
        // so the model can never choose whom to search for.
        [McpServerTool(Name = "search_cleaners", ReadOnly = true)]
        [Authorize(Roles = "customer")]
        [Description("Search for cleaners available at a specific date and time for the logged-in customer. " +
                     "Returns up to 5 cleaners sorted by match score (distance and rating), with the total price. " +
                     "Use it only after the customer gave a date, a start time and a duration.")]
        public async Task<string> SearchCleaners(
            ClaimsPrincipal user,
            [Description("Date in the format yyyy-MM-dd. Today or later.")] string date,
            [Description("Start time in the format HH:mm, 24-hour clock.")] string start,
            [Description("Duration in hours, for example 2.5.")] double hours,
            [Description("Highest price per hour in ILS. Optional.")] int? maxPricePerHour = null)
        {
            var userId = user.GetUserId();
            var customer = userId == null ? null : await _customers.GetCustomerByUserIdAsync(userId.Value);
            if (customer == null)
            {
                throw new McpException("Only a logged-in customer can search for cleaners.");
            }

            var (day, startTime) = ParseWhen(date, start);

            try
            {
                var matches = await _search.SearchAsync(customer, day, startTime, hours, maxPricePerHour);
                var results = _mapper.Map<List<CleanerSearchResultDTO>>(matches.Take(MaxResults).ToList());
                return JsonSerializer.Serialize(results, JsonOptions);
            }
            catch (Exception error) when (error is ArgumentException or InvalidOperationException)
            {
                // The search rules (a past time, no address on file, ...) are explained to the model so it can fix its request.
                throw new McpException(error.Message);
            }
        }

        // The model writes dates and times as text, so they are checked here with a message it can act on.
        public static (DateOnly Date, TimeOnly Start) ParseWhen(string date, string start)
        {
            if (!DateOnly.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var day))
            {
                throw new McpException($"The date '{date}' is not valid. Use the format yyyy-MM-dd, for example 2026-11-25.");
            }

            if (!TimeOnly.TryParseExact(start, "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var startTime))
            {
                throw new McpException($"The start time '{start}' is not valid. Use the 24-hour format HH:mm, for example 09:30.");
            }

            return (day, startTime);
        }
    }
}
