using Clean.Core.Agent;
using Clean.Core.Llm;

namespace Clean.Service
{
    public class SearchAgent : ISearchAgent
    {
        private const int MaxSteps = 6;
        private const int MaxHistoryMessages = 40;
        private const int MaxTextLength = 500;
        private const string GiveUpText = "לא הצלחתי להשלים את הבקשה. אפשר לנסח אותה אחרת?";

        private readonly ILlmClient _llm;
        private readonly IToolSessionFactory _tools;
        private readonly TimeProvider _timeProvider;

        public SearchAgent(ILlmClient llm, IToolSessionFactory tools, TimeProvider timeProvider)
        {
            _llm = llm;
            _tools = tools;
            _timeProvider = timeProvider;
        }

        public async Task<AgentTurn> RunAsync(
            IReadOnlyList<LlmMessage> history, string userMessage, string userToken, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(userMessage))
            {
                throw new ArgumentException("כתבי מה את מחפשת.");
            }
            if (userMessage.Length > MaxTextLength)
            {
                throw new ArgumentException($"ההודעה ארוכה מדי (עד {MaxTextLength} תווים).");
            }
            if (history.Count > MaxHistoryMessages)
            {
                throw new ArgumentException("השיחה ארוכה מדי. התחילי שיחה חדשה.");
            }

            var today = DateOnly.FromDateTime(IsraelTime.Now(_timeProvider));
            var system = AgentPrompt.Build(today);

            await using var session = await _tools.OpenAsync(userToken, ct);
            var tools = await session.ListToolsAsync(ct);

            var added = new List<LlmMessage> { new("user", userMessage) };

            // A ceiling on the loop: a confused model could ask for tools forever, and every round costs time and quota.
            for (var step = 0; step < MaxSteps; step++)
            {
                var conversation = history.Concat(added).ToList();
                var reply = await _llm.SendAsync(conversation, tools, system, ct);

                if (!reply.HasToolCalls)
                {
                    return string.IsNullOrWhiteSpace(reply.Text)
                        ? GiveUp(added)
                        : Finish(added, reply.Text);
                }

                // All the requests first, then all the results, which is the order Gemini expects.
                foreach (var call in reply.ToolCalls!)
                {
                    added.Add(new LlmMessage("assistant", "", call));
                }
                foreach (var call in reply.ToolCalls!)
                {
                    var result = await session.CallAsync(call.Name, call.ArgumentsJson, ct);
                    added.Add(new LlmMessage("tool", result.Text, ToolName: call.Name));
                }
            }

            return GiveUp(added);
        }

        private static AgentTurn Finish(List<LlmMessage> added, string text)
        {
            added.Add(new LlmMessage("assistant", text));
            return new AgentTurn(text, added);
        }

        private static AgentTurn GiveUp(List<LlmMessage> added) => Finish(added, GiveUpText);
    }
}
