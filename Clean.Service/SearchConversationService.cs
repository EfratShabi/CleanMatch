using Clean.Core.Entities;
using Clean.Core.Models;
using Clean.Core.Services;

namespace Clean.Service
{
    public class SearchConversationService : ISearchConversationService
    {
        private readonly ISearchIntentService _intents;
        private readonly ICleanerSearchService _search;

        public SearchConversationService(ISearchIntentService intents, ICleanerSearchService search)
        {
            _intents = intents;
            _search = search;
        }

        public async Task<SearchConversationResult> HandleAsync(
            Customers customer, string text, SearchIntent? known, SearchField? asked, CancellationToken ct = default)
        {
            var before = known ?? new SearchIntent(null, null, null, null);

            var understood = await _intents.InterpretAsync(text, asked, ct);
            if (understood is null)
            {
                return new(SearchConversationStatus.NotUnderstood, before, before.NextMissingField, null);
            }

            var intent = before.MergeWith(understood);
            if (intent.NextMissingField is { } next)
            {
                return new(SearchConversationStatus.NeedsMore, intent, next, null);
            }

            var matches = await _search.SearchAsync(customer, intent.Date!.Value, intent.Start!.Value, intent.Hours!.Value, intent.MaxPrice);
            return new(SearchConversationStatus.Results, intent, null, matches);
        }
    }
}
