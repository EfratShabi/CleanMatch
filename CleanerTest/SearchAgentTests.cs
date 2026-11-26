using Clean.Core.Agent;
using Clean.Core.Llm;
using Clean.Service;

namespace CleanerTest
{
    public class SearchAgentTests
    {
        private sealed class FixedTimeProvider : TimeProvider
        {
            private readonly DateTimeOffset _utcNow;

            public FixedTimeProvider(DateTimeOffset utcNow) => _utcNow = utcNow;

            public override DateTimeOffset GetUtcNow() => _utcNow;
        }

        // Answers with the scripted replies in order; once they run out it keeps repeating the last one.
        private sealed class ScriptedLlm : ILlmClient
        {
            private readonly Queue<LlmReply> _replies;
            private LlmReply _last = null!;

            public List<List<LlmMessage>> Conversations { get; } = new();
            public List<IReadOnlyList<LlmTool>?> ToolLists { get; } = new();
            public List<string?> SystemInstructions { get; } = new();

            public ScriptedLlm(params LlmReply[] replies) => _replies = new Queue<LlmReply>(replies);

            public Task<LlmReply> SendAsync(
                IReadOnlyList<LlmMessage> conversation, IReadOnlyList<LlmTool>? tools = null,
                string? systemInstruction = null, CancellationToken ct = default)
            {
                Conversations.Add(conversation.ToList());
                ToolLists.Add(tools);
                SystemInstructions.Add(systemInstruction);
                if (_replies.Count > 0) _last = _replies.Dequeue();
                return Task.FromResult(_last);
            }
        }

        private sealed class FakeSession : IToolSession
        {
            private readonly ToolResult _result;

            public List<(string Name, string Arguments)> Calls { get; } = new();
            public bool Disposed { get; private set; }

            public FakeSession(ToolResult result) => _result = result;

            public Task<IReadOnlyList<LlmTool>> ListToolsAsync(CancellationToken ct = default) =>
                Task.FromResult<IReadOnlyList<LlmTool>>(new[] { SearchTool });

            public Task<ToolResult> CallAsync(string name, string argumentsJson, CancellationToken ct = default)
            {
                Calls.Add((name, argumentsJson));
                return Task.FromResult(_result);
            }

            public ValueTask DisposeAsync()
            {
                Disposed = true;
                return ValueTask.CompletedTask;
            }
        }

        private sealed class FakeFactory : IToolSessionFactory
        {
            public FakeSession Session { get; }
            public string? TokenUsed { get; private set; }
            public int Opened { get; private set; }

            public FakeFactory(ToolResult result) => Session = new FakeSession(result);

            public Task<IToolSession> OpenAsync(string userToken, CancellationToken ct = default)
            {
                Opened++;
                TokenUsed = userToken;
                return Task.FromResult<IToolSession>(Session);
            }
        }

        private static readonly LlmTool SearchTool = new("search_cleaners", "Search.", """{"type":"object"}""");
        private static readonly LlmToolCall SearchCall = new("search_cleaners", """{"date":"2026-11-24"}""");

        private static LlmReply Text(string text) => new(text, 0, 0);
        private static LlmReply AsksForTool() => new("", 0, 0, new[] { SearchCall });

        private static SearchAgent CreateAgent(ILlmClient llm, FakeFactory factory) =>
            new(llm, factory, new FixedTimeProvider(DateTimeOffset.Parse("2026-11-23T10:00:00Z")));

        private static FakeFactory Factory(string result = "[]", bool isError = false) =>
            new(new ToolResult(result, isError));

        [Fact]
        public async Task RunAsync_returns_a_plain_answer_without_running_any_tool()
        {
            var factory = Factory();
            var llm = new ScriptedLlm(Text("לאיזה יום?"));

            var turn = await CreateAgent(llm, factory).RunAsync(new List<LlmMessage>(), "מחפשת מנקה", "token-1");

            Assert.Equal("לאיזה יום?", turn.Text);
            Assert.Empty(factory.Session.Calls);
            Assert.Equal(new[] { "user", "assistant" }, turn.NewMessages.Select(m => m.Role));
        }

        [Fact]
        public async Task RunAsync_runs_the_requested_tool_and_gives_the_result_back_to_the_model()
        {
            var factory = Factory("""[{"id":34}]""");
            var llm = new ScriptedLlm(AsksForTool(), Text("מצאתי מנקה אחת."));

            var turn = await CreateAgent(llm, factory).RunAsync(new List<LlmMessage>(), "מחר ב-9 ל-3 שעות", "token-1");

            Assert.Equal("מצאתי מנקה אחת.", turn.Text);
            Assert.Equal(("search_cleaners", """{"date":"2026-11-24"}"""), Assert.Single(factory.Session.Calls));

            var secondConversation = llm.Conversations[1];
            Assert.Equal(new[] { "user", "assistant", "tool" }, secondConversation.Select(m => m.Role));
            Assert.Equal(SearchCall, secondConversation[1].ToolCall);
            Assert.Equal("""[{"id":34}]""", secondConversation[2].Text);
            Assert.Equal("search_cleaners", secondConversation[2].ToolName);

            Assert.Equal(new[] { "user", "assistant", "tool", "assistant" }, turn.NewMessages.Select(m => m.Role));
        }

        [Fact]
        public async Task RunAsync_passes_a_tool_error_to_the_model_so_it_can_react()
        {
            var factory = Factory("תאריך שעבר", isError: true);
            var llm = new ScriptedLlm(AsksForTool(), Text("התאריך עבר, לאיזה יום להזמין?"));

            var turn = await CreateAgent(llm, factory).RunAsync(new List<LlmMessage>(), "אתמול", "token-1");

            Assert.Equal("תאריך שעבר", llm.Conversations[1].Last().Text);
            Assert.Equal("התאריך עבר, לאיזה יום להזמין?", turn.Text);
        }

        [Fact]
        public async Task RunAsync_stops_after_six_rounds_when_the_model_keeps_asking_for_tools()
        {
            var factory = Factory();
            var llm = new ScriptedLlm(AsksForTool());

            var turn = await CreateAgent(llm, factory).RunAsync(new List<LlmMessage>(), "מחפשת מנקה", "token-1");

            Assert.Equal(6, llm.Conversations.Count);
            Assert.Equal(6, factory.Session.Calls.Count);
            Assert.Contains("לא הצלחתי", turn.Text);
        }

        [Fact]
        public async Task RunAsync_gives_up_politely_when_the_model_answers_with_nothing()
        {
            var turn = await CreateAgent(new ScriptedLlm(Text("  ")), Factory())
                .RunAsync(new List<LlmMessage>(), "מחפשת מנקה", "token-1");

            Assert.Contains("לא הצלחתי", turn.Text);
        }

        [Fact]
        public async Task RunAsync_sends_todays_date_the_tools_and_the_history_to_the_model()
        {
            var llm = new ScriptedLlm(Text("בסדר"));
            var history = new List<LlmMessage> { new("user", "היי"), new("assistant", "שלום!") };

            await CreateAgent(llm, Factory()).RunAsync(history, "מחר", "token-1");

            Assert.Contains("Today is 2026-11-23 (Monday)", llm.SystemInstructions[0]);
            Assert.Equal("search_cleaners", Assert.Single(llm.ToolLists[0]!).Name);
            Assert.Equal(new[] { "היי", "שלום!", "מחר" }, llm.Conversations[0].Select(m => m.Text));
        }

        [Fact]
        public async Task RunAsync_opens_the_session_with_the_customers_own_token_and_closes_it()
        {
            var factory = Factory();

            await CreateAgent(new ScriptedLlm(Text("בסדר")), factory).RunAsync(new List<LlmMessage>(), "היי", "the-customers-token");

            Assert.Equal("the-customers-token", factory.TokenUsed);
            Assert.True(factory.Session.Disposed);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task RunAsync_rejects_an_empty_message_before_doing_anything(string message)
        {
            var factory = Factory();

            await Assert.ThrowsAsync<ArgumentException>(() =>
                CreateAgent(new ScriptedLlm(Text("x")), factory).RunAsync(new List<LlmMessage>(), message, "token-1"));

            Assert.Equal(0, factory.Opened);
        }

        [Fact]
        public async Task RunAsync_rejects_a_message_over_500_characters()
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
                CreateAgent(new ScriptedLlm(Text("x")), Factory())
                    .RunAsync(new List<LlmMessage>(), new string('א', 501), "token-1"));
        }

        [Fact]
        public async Task RunAsync_rejects_a_history_that_is_too_long()
        {
            var history = Enumerable.Range(0, 41).Select(i => new LlmMessage("user", "x")).ToList();

            await Assert.ThrowsAsync<ArgumentException>(() =>
                CreateAgent(new ScriptedLlm(Text("x")), Factory()).RunAsync(history, "היי", "token-1"));
        }
    }
}
