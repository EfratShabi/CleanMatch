namespace Clean.Service
{
    public static class CleanerMatching
    {
        public const double RatingWeight = 0.6;
        public const double DistanceWeight = 0.4;
        public const double PriorRating = 3.5;
        public const int PriorWeight = 5;

        // A cleaner with no reviews starts at the prior; every review pulls her toward her real average.
        public static double AdjustedRating(double ratingSum, int reviewCount) =>
            (PriorWeight * PriorRating + ratingSum) / (PriorWeight + reviewCount);

        public static double Score(double ratingSum, int reviewCount, double distanceKm, int upToKm)
        {
            var ratingScore = (AdjustedRating(ratingSum, reviewCount) - 1) / 4;
            var distanceScore = upToKm > 0 ? Math.Clamp(1 - distanceKm / upToKm, 0, 1) : 1;
            return Math.Round(100 * (RatingWeight * ratingScore + DistanceWeight * distanceScore), 1);
        }
    }
}
