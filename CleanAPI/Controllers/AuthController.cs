using Clean.Core.DTO;
using Clean.Core.Services;
using CleanAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CleanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        private readonly ICleanerService _cleanerService;
        private readonly ICustomerService _customerService;
        public AuthController(IUserService userService, IConfiguration configuration,
            ICleanerService cleanerService, ICustomerService customerService)
        {
            _userService = userService;
            _configuration = configuration;
            _cleanerService = cleanerService;
            _customerService = customerService;
        }
        [HttpPost("login")] 
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            var user = await _userService.GetByUserNameAsync(loginModel.UserName, loginModel.Password);
            if (user != null)
            {
                var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };
                var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
                var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                var tokeOptions = new JwtSecurityToken(
                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:Audience"],
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(60),
                    signingCredentials: signinCredentials
                );
                var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                return Ok(new { Token = tokenString });
            }
            return Unauthorized();
        }

        // "Who am I?" - any logged-in user may call it; it only ever returns the user of the token,
        // so there is no way to ask about someone else.
        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<MeDTO>> Me()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (!int.TryParse(userIdClaim, out var userId) || role == null)
            {
                return Unauthorized();
            }

            int? cleanerId = null;
            int? customerId = null;

            if (role == "cleaner")
            {
                var cleaner = await _cleanerService.GetCleanerByUserIdAsync(userId);
                if (cleaner == null) return Unauthorized(); // valid token but the profile no longer exists
                cleanerId = cleaner.Id;
            }
            else if (role == "customer")
            {
                var customer = await _customerService.GetCustomerByUserIdAsync(userId);
                if (customer == null) return Unauthorized();
                customerId = customer.Id;
            }
            // manager: no profile, both ids stay null

            return Ok(new MeDTO
            {
                UserId = userId,
                UserName = User.FindFirstValue(ClaimTypes.Name) ?? string.Empty,
                Role = role,
                CleanerId = cleanerId,
                CustomerId = customerId
            });
        }

    }
}
