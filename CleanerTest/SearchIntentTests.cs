using Clean.Core.Models;

namespace CleanerTest
{
    public class SearchIntentTests
    {
        private static readonly DateOnly SomeDate = new(2026, 11, 24);
        private static readonly TimeOnly SomeStart = new(8, 0);

        [Fact]
        public void NextMissingField_is_null_when_everything_required_is_present()
        {
            Assert.Null(new SearchIntent(SomeDate, SomeStart, 3, null).NextMissingField);
        }

        [Fact]
        public void NextMissingField_is_Date_when_the_date_is_missing()
        {
            Assert.Equal(SearchField.Date, new SearchIntent(null, SomeStart, 3, 80).NextMissingField);
        }

        [Fact]
        public void NextMissingField_is_Start_when_the_start_time_is_missing()
        {
            Assert.Equal(SearchField.Start, new SearchIntent(SomeDate, null, 3, 80).NextMissingField);
        }

        [Fact]
        public void NextMissingField_is_Hours_when_the_duration_is_missing()
        {
            Assert.Equal(SearchField.Hours, new SearchIntent(SomeDate, SomeStart, null, 80).NextMissingField);
        }

        [Fact]
        public void NextMissingField_returns_the_first_missing_one_in_order()
        {
            Assert.Equal(SearchField.Start, new SearchIntent(SomeDate, null, null, 80).NextMissingField);
            Assert.Equal(SearchField.Date, new SearchIntent(null, null, null, null).NextMissingField);
        }
    }
}
