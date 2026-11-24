using Clean.Core.Entities;
using Clean.Core.Models;
using Clean.Core.Services;
using Clean.Service;

namespace CleanerTest
{
    public class SearchConversationServiceTests
    {
        private static readonly DateOnly Tomorrow = new(2026, 11, 24);
        private static readonly TimeOnly Morning = new(8, 0);

        private sealed class FakeIntents : ISearchIntentService
        {
            private readonly SearchIntent? _answer;

            public string? LastText { get; private set; }
            public SearchField? LastAsked { get; private set; }

            public FakeIntents(SearchIntent? answer) => _answer = answer;

            public Task<SearchIntent?> InterpretAsync(string customerText, SearchField? asked = null, CancellationToken ct = default)
            {
                LastText = customerText;
                LastAsked = asked;
                return Task.FromResult(_answer);
            }
        }

        private sealed class FakeSearch : ICleanerSearchService
        {
            public List<CleanerMatch> Matches { get; } = new();
            public int Calls { get; private set; }
            public (DateOnly Date, TimeOnly Start, double Hours, int? MaxPrice)? LastArgs { get; private set; }

            public Task<List<CleanerMatch>> SearchAsync(Customers customer, DateOnly date, TimeOnly start, double hours, int? maxPricePerHour)
            {
                Calls++;
                LastArgs = (date, start, hours, maxPricePerHour);
                return Task.FromResult(Matches);
            }
        }

        private static SearchIntent Intent(DateOnly? date, TimeOnly? start, double? hours, int? maxPrice = null) =>
            new(date, start, hours, maxPrice);

        [Fact]
        public async Task HandleAsync_reports_not_understood_and_keeps_what_was_known()
        {
            var known = Intent(Tomorrow, null, null);
            var service = new SearchConversationService(new FakeIntents(null), new FakeSearch());

            var result = await service.HandleAsync(new Customers(), "???", known, null);

            Assert.Equal(SearchConversationStatus.NotUnderstood, result.Status);
            Assert.Equal(known, result.Intent);
            Assert.Equal(SearchField.Start, result.NextField);
            Assert.Null(result.Matches);
        }

        [Fact]
        public async Task HandleAsync_asks_for_the_next_missing_detail()
        {
            var service = new SearchConversationService(new FakeIntents(Intent(Tomorrow, null, null)), new FakeSearch());

            var result = await service.HandleAsync(new Customers(), "tomorrow", null, null);

            Assert.Equal(SearchConversationStatus.NeedsMore, result.Status);
            Assert.Equal(SearchField.Start, result.NextField);
            Assert.Equal(Intent(Tomorrow, null, null), result.Intent);
        }

        [Fact]
        public async Task HandleAsync_does_not_search_while_something_is_missing()
        {
            var search = new FakeSearch();
            var service = new SearchConversationService(new FakeIntents(Intent(Tomorrow, Morning, null)), search);

            await service.HandleAsync(new Customers(), "tomorrow morning", null, null);

            Assert.Equal(0, search.Calls);
        }

        [Fact]
        public async Task HandleAsync_merges_the_answer_with_what_was_known_and_searches()
        {
            var search = new FakeSearch();
            var known = Intent(Tomorrow, Morning, null, 80);
            var service = new SearchConversationService(new FakeIntents(Intent(null, null, 3)), search);

            var result = await service.HandleAsync(new Customers(), "3", known, SearchField.Hours);

            Assert.Equal(SearchConversationStatus.Results, result.Status);
            Assert.Null(result.NextField);
            Assert.Equal(Intent(Tomorrow, Morning, 3, 80), result.Intent);
            Assert.Equal((Tomorrow, Morning, 3.0, (int?)80), search.LastArgs);
            Assert.Same(search.Matches, result.Matches);
        }

        [Fact]
        public async Task HandleAsync_passes_the_text_and_the_pending_question_to_the_interpreter()
        {
            var intents = new FakeIntents(Intent(null, null, 3));
            var service = new SearchConversationService(intents, new FakeSearch());

            await service.HandleAsync(new Customers(), "3", null, SearchField.Hours);

            Assert.Equal("3", intents.LastText);
            Assert.Equal(SearchField.Hours, intents.LastAsked);
        }
    }
}
