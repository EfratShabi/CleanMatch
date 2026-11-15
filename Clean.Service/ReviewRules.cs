namespace Clean.Service
{
    public static class ReviewRules
    {
        public const int MinRating = 1;
        public const int MaxRating = 5;
        public const int MaxTextLength = 1000;

        public static void Validate(int rating, string? text)
        {
            if (rating < MinRating || rating > MaxRating)
                throw new ArgumentException("הדירוג חייב להיות בין 1 ל-5.");

            if (text != null && text.Length > MaxTextLength)
                throw new ArgumentException($"הטקסט ארוך מדי (עד {MaxTextLength} תווים).");
        }

        public static bool ContainsBadWord(string? text, IEnumerable<string> badWords) =>
            !string.IsNullOrWhiteSpace(text)
            && badWords.Any(w => text.Contains(w, StringComparison.OrdinalIgnoreCase));
    }
}
