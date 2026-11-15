namespace Clean.Core.DTO
{
    // The answer to "who am I?" - returned by GET /api/auth/me.
    // UserId is the Id in the Users table; CleanerId / CustomerId are the Ids of the profile tables.
    public class MeDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int? CleanerId { get; set; }
        public int? CustomerId { get; set; }
    }
}
