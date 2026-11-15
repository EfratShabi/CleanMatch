namespace Clean.Core.DTO
{
    public class CleanerSearchResultDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string DescriptionOnMe { get; set; }
        public bool BornIsrael { get; set; }
        public bool HebrewSpeaker { get; set; }
        public int PricePerHour { get; set; }
        public double TotalPrice { get; set; }
        public double? AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public double DistanceKm { get; set; }
        public double MatchScore { get; set; }
    }
}
