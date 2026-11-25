using Clean.Core.Llm;
using Clean.Core.Models;
using Clean.Service;

namespace CleanerTest
{
    public class SearchIntentServiceTests
    {
        private sealed class FixedTimeProvider : TimeProvider
        {
            private readonly DateTimeOffset _utcNow;

            public FixedTimeProvider(DateTimeOffset utcNow) => _utcNow = utcNow;

            public override DateTimeOffset GetUtcNow() => _utcNow;
        }

        private sealed class FakeLlm : ILlmClient
        {
            private readonly Func<LlmReply> _reply;

            public List<IReadOnlyList<LlmMessage>> Conversations { get; } = new();

            public FakeLlm(Func<LlmReply> reply) => _reply = reply;

            public Task<LlmReply> SendAsync(
                IReadOnlyList<LlmMessage> conversation, IReadOnlyList<LlmTool>? tools = null, CancellationToken ct = default)
            {
                Conversations.Add(conversation);
                return Task.FromResult(_reply());
            }
        }

        private static LlmReply Reply(string text) => new(text, 0, 0);

        private static SearchIntentService CreateService(FakeLlm llm) =>
            new(llm, new FixedTimeProvider(DateTimeOffset.Parse("2026-11-23T10:00:00Z")));

        [Fact]
        public async Task InterpretAsync_returns_the_parsed_intent()
        {
            var llm = new FakeLlm(() => Reply("""{"date":"2026-11-24","start":"08:00","hours":3,"maxPrice":80}"""));

            var intent = await CreateService(llm).InterpretAsync("מנקה מחר בבוקר ל-3 שעות עד 80 שקל");

            Assert.Equal(new SearchIntent(new DateOnly(2026, 11, 24), new TimeOnly(8, 0), 3, 80), intent);
        }

        [Fact]
        public async Task InterpretAsync_sends_one_user_message_with_todays_date_and_the_customer_text()
        {
            var llm = new FakeLlm(() => Reply("{}"));

            await CreateService(llm).InterpretAsync("מנקה מחר בבוקר");

            var sent = Assert.Single(Assert.Single(llm.Conversations));
            Assert.Equal("user", sent.Role);
            Assert.Contains("Today is 2026-11-23 (Monday)", sent.Text);
            Assert.EndsWith("Customer request:\nמנקה מחר בבוקר", sent.Text);
        }

        [Fact]
        public async Task InterpretAsync_tells_the_model_which_question_was_asked()
        {
            var llm = new FakeLlm(() => Reply("""{"hours":3}"""));

            var intent = await CreateService(llm).InterpretAsync("3", SearchField.Hours);

            Assert.Equal(3, intent!.Hours);
            var sent = Assert.Single(Assert.Single(llm.Conversations));
            Assert.Contains("was just asked for: Hours.", sent.Text);
            Assert.EndsWith("Customer request:\n3", sent.Text);
        }

        [Fact]
        public async Task InterpretAsync_returns_an_empty_intent_when_the_customer_gave_nothing_usable()
        {
            var llm = new FakeLlm(() => Reply("""{"date":null,"start":null,"hours":null,"maxPrice":null}"""));

            var intent = await CreateService(llm).InterpretAsync("שלום");

            Assert.Equal(new SearchIntent(null, null, null, null), intent);
        }

        [Fact]
        public async Task InterpretAsync_returns_null_when_the_answer_cannot_be_read()
        {
            var llm = new FakeLlm(() => Reply("I don't understand your request"));

            Assert.Null(await CreateService(llm).InterpretAsync("מנקה מחר"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task InterpretAsync_rejects_empty_text_without_calling_the_model(string text)
        {
            var llm = new FakeLlm(() => Reply("{}"));

            await Assert.ThrowsAsync<ArgumentException>(() => CreateService(llm).InterpretAsync(text));

            Assert.Empty(llm.Conversations);
        }

        [Fact]
        public async Task InterpretAsync_rejects_text_over_500_characters_but_accepts_exactly_500()
        {
            var llm = new FakeLlm(() => Reply("{}"));
            var service = CreateService(llm);

            await Assert.ThrowsAsync<ArgumentException>(() => service.InterpretAsync(new string('א', 501)));
            Assert.Empty(llm.Conversations);

            await service.InterpretAsync(new string('א', 500));
            Assert.Single(llm.Conversations);
        }

        [Fact]
        public async Task InterpretAsync_lets_a_model_failure_pass_through()
        {
            var llm = new FakeLlm(() => throw new LlmException("down", isTemporary: true));

            var error = await Assert.ThrowsAsync<LlmException>(() => CreateService(llm).InterpretAsync("מנקה מחר"));

            Assert.True(error.IsTemporary);
        }
    }
}
