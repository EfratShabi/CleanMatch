using CleanAPI.Mcp;
using ModelContextProtocol;

namespace CleanerTest
{
    public class CleanerToolsTests
    {
        [Fact]
        public void ParseWhen_reads_a_date_and_a_24_hour_time()
        {
            var (date, start) = CleanerTools.ParseWhen("2026-11-25", "09:30");

            Assert.Equal(new DateOnly(2026, 11, 25), date);
            Assert.Equal(new TimeOnly(9, 30), start);
        }

        [Theory]
        [InlineData("אתמול", "09:00")]
        [InlineData("tomorrow", "09:00")]
        [InlineData("25/11/2026", "09:00")]
        [InlineData("2026-13-40", "09:00")]
        [InlineData("", "09:00")]
        public void ParseWhen_rejects_a_date_that_is_not_yyyy_MM_dd_with_a_message_naming_the_format(string date, string start)
        {
            var error = Assert.Throws<McpException>(() => CleanerTools.ParseWhen(date, start));

            Assert.Contains("yyyy-MM-dd", error.Message);
        }

        [Theory]
        [InlineData("9am")]
        [InlineData("25:00")]
        [InlineData("9:30 PM")]
        [InlineData("")]
        public void ParseWhen_rejects_a_time_that_is_not_HH_mm_with_a_message_naming_the_format(string start)
        {
            var error = Assert.Throws<McpException>(() => CleanerTools.ParseWhen("2026-11-25", start));

            Assert.Contains("HH:mm", error.Message);
        }
    }
}
