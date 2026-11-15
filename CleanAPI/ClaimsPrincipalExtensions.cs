using System.Security.Claims;

namespace CleanAPI
{
    // Small helpers for reading the logged-in user out of the JWT token (the "User" property inside a Controller).
    public static class ClaimsPrincipalExtensions
    {
        // The Id of the logged-in user in the Users table (the NameIdentifier claim we put in the token at login).
        // Returns null if the claim is missing or is not a number.
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(value, out var id) ? id : null;
        }

        public static bool IsManager(this ClaimsPrincipal user) => user.IsInRole("manager");
    }
}
