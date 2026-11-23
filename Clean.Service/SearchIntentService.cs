using Clean.Core.Llm;
using Clean.Core.Models;
using Clean.Core.Services;

namespace Clean.Service
{
    public class SearchIntentService : ISearchIntentService
    {
        private const int MaxTextLength = 500;

        private readonly ILlmClient _llm;
        private readonly TimeProvider _timeProvider;

        public SearchIntentService(ILlmClient llm, TimeProvider timeProvider)
        {
            _llm = llm;
            _timeProvider = timeProvider;
        }

        public async Task<SearchIntent?> InterpretAsync(string customerText, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(customerText))
            {
                throw new ArgumentException("כתבי מה את מחפשת.");
            }

            if (customerText.Length > MaxTextLength)
            {
                throw new ArgumentException($"הבקשה ארוכה מדי (עד {MaxTextLength} תווים).");
            }

            var today = DateOnly.FromDateTime(IsraelTime.Now(_timeProvider));
            var message = SearchIntentPrompt.Build(today) + "\n\nCustomer request:\n" + customerText;

            var reply = await _llm.SendAsync(new[] { new LlmMessage("user", message) }, ct);

            return SearchIntentParser.Parse(reply.Text);
        }
    }
}
