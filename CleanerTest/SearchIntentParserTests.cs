using Clean.Core.Models;
using Clean.Service;

namespace CleanerTest
{
    public class SearchIntentParserTests
    {
        private static readonly SearchIntent Full = new(new DateOnly(2026, 11, 24), new TimeOnly(8, 0), 3, 80);

        [Fact]
        public void Parse_reads_a_clean_json_answer()
        {
            var intent = SearchIntentParser.Parse("""{"date":"2026-11-24","start":"08:00","hours":3,"maxPrice":80}""");

            Assert.Equal(Full, intent);
        }

        [Fact]
        public void Parse_finds_the_json_inside_code_fences_and_chatter()
        {
            var text = """
                Sure! Here is the JSON:
                ```json
                {"date":"2026-11-24","start":"08:00","hours":3,"maxPrice":80}
                ```
                Hope that helps.
                """;

            Assert.Equal(Full, SearchIntentParser.Parse(text));
        }

        [Fact]
        public void Parse_treats_null_and_missing_fields_as_not_given()
        {
            var intent = SearchIntentParser.Parse("""{"date":"2026-11-24","start":null,"hours":null}""");

            Assert.Equal(new SearchIntent(new DateOnly(2026, 11, 24), null, null, null), intent);
        }

        [Fact]
        public void Parse_returns_an_empty_intent_for_an_empty_object()
        {
            Assert.Equal(new SearchIntent(null, null, null, null), SearchIntentParser.Parse("{}"));
        }

        [Fact]
        public void Parse_accepts_fractional_hours()
        {
            var intent = SearchIntentParser.Parse("""{"hours":2.5}""");

            Assert.Equal(2.5, intent!.Hours);
        }

        [Theory]
        [InlineData("I don't understand your request")]
        [InlineData("")]
        [InlineData("""{"date": }""")]
        [InlineData("""{"date":"מחר"}""")]
        [InlineData("""{"date":20261124}""")]
        [InlineData("""{"start":"8am"}""")]
        [InlineData("""{"start":"25:00"}""")]
        [InlineData("""{"hours":"three"}""")]
        [InlineData("""{"hours":0}""")]
        [InlineData("""{"hours":-2}""")]
        [InlineData("""{"maxPrice":-50}""")]
        [InlineData("""{"maxPrice":"eighty"}""")]
        public void Parse_returns_null_when_the_answer_is_unusable(string text)
        {
            Assert.Null(SearchIntentParser.Parse(text));
        }
    }
}
