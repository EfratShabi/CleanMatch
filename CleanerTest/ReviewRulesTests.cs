using Clean.Service;

namespace CleanerTest
{
    public class ReviewRulesTests
    {
        [Theory]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        public void Validate_accepts_ratings_from_1_to_5(int rating)
        {
            ReviewRules.Validate(rating, "great");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        [InlineData(-1)]
        public void Validate_rejects_ratings_outside_1_to_5(int rating)
        {
            Assert.Throws<ArgumentException>(() => ReviewRules.Validate(rating, "great"));
        }

        [Fact]
        public void Validate_allows_a_review_without_text()
        {
            ReviewRules.Validate(4, null);
            ReviewRules.Validate(4, "");
        }

        [Fact]
        public void Validate_accepts_text_of_exactly_the_maximum_length_and_rejects_one_more()
        {
            ReviewRules.Validate(4, new string('a', ReviewRules.MaxTextLength));
            Assert.Throws<ArgumentException>(() =>
                ReviewRules.Validate(4, new string('a', ReviewRules.MaxTextLength + 1)));
        }

        [Fact]
        public void ContainsBadWord_finds_a_word_inside_a_sentence()
        {
            Assert.True(ReviewRules.ContainsBadWord("this is badword here", new[] { "badword" }));
        }

        [Fact]
        public void ContainsBadWord_ignores_letter_case()
        {
            Assert.True(ReviewRules.ContainsBadWord("BADWORD", new[] { "badword" }));
        }

        [Fact]
        public void ContainsBadWord_is_false_for_clean_text()
        {
            Assert.False(ReviewRules.ContainsBadWord("lovely and clean", new[] { "badword" }));
        }

        [Fact]
        public void ContainsBadWord_is_false_for_empty_or_missing_text_and_empty_list()
        {
            Assert.False(ReviewRules.ContainsBadWord(null, new[] { "badword" }));
            Assert.False(ReviewRules.ContainsBadWord("   ", new[] { "badword" }));
            Assert.False(ReviewRules.ContainsBadWord("anything", Array.Empty<string>()));
        }
    }
}
