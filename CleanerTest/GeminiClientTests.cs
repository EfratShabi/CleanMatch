using Clean.Core.Llm;
using Clean.Service;
using System.Net;
using System.Text;

namespace CleanerTest
{
    public class GeminiClientTests
    {
        private const string OkJson =
            """{ "candidates": [ { "content": { "parts": [ { "text": "Hello" } ] } } ], "usageMetadata": { "promptTokenCount": 3, "candidatesTokenCount": 4 } }""";

        private record CapturedRequest(string Url, string? ApiKeyHeader, string Body);

        private class FakeHandler : HttpMessageHandler
        {
            private readonly Func<HttpResponseMessage> _respond;

            public List<CapturedRequest> Requests { get; } = new();

            public FakeHandler(Func<HttpResponseMessage> respond)
            {
                _respond = respond;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
            {
                request.Headers.TryGetValues("x-goog-api-key", out var keys);
                var body = request.Content == null ? "" : await request.Content.ReadAsStringAsync(ct);
                Requests.Add(new CapturedRequest(request.RequestUri!.AbsoluteUri, keys?.FirstOrDefault(), body));
                return _respond();
            }
        }

        private static HttpResponseMessage Reply(HttpStatusCode status, string body) =>
            new(status) { Content = new StringContent(body, Encoding.UTF8, "application/json") };

        private static GeminiClient CreateClient(FakeHandler handler, string apiKey = "test-key") =>
            new(
                new HttpClient(handler) { BaseAddress = new Uri("https://example.test/v1beta/") },
                new GeminiSettings(apiKey, "test-model", TimeSpan.Zero));

        private static readonly List<LlmMessage> Conversation = new() { new("user", "Say hello") };

        [Fact]
        public async Task SendAsync_returns_the_parsed_reply_on_success()
        {
            var handler = new FakeHandler(() => Reply(HttpStatusCode.OK, OkJson));

            var reply = await CreateClient(handler).SendAsync(Conversation);

            Assert.Equal("Hello", reply.Text);
            Assert.Equal(3, reply.InputTokens);
            Assert.Equal(4, reply.OutputTokens);
        }

        [Fact]
        public async Task SendAsync_sends_the_model_url_the_key_header_and_the_conversation()
        {
            var handler = new FakeHandler(() => Reply(HttpStatusCode.OK, OkJson));

            await CreateClient(handler).SendAsync(Conversation);

            var sent = Assert.Single(handler.Requests);
            Assert.Equal("https://example.test/v1beta/models/test-model:generateContent", sent.Url);
            Assert.Equal("test-key", sent.ApiKeyHeader);
            Assert.Contains("Say hello", sent.Body);
        }

        [Theory]
        [InlineData(HttpStatusCode.RequestTimeout, true)]
        [InlineData(HttpStatusCode.TooManyRequests, true)]
        [InlineData(HttpStatusCode.InternalServerError, true)]
        [InlineData(HttpStatusCode.ServiceUnavailable, true)]
        [InlineData(HttpStatusCode.BadRequest, false)]
        [InlineData(HttpStatusCode.Unauthorized, false)]
        [InlineData(HttpStatusCode.Forbidden, false)]
        [InlineData(HttpStatusCode.NotFound, false)]
        public async Task SendAsync_marks_errors_as_temporary_or_permanent_by_status_code(HttpStatusCode status, bool expectedTemporary)
        {
            var handler = new FakeHandler(() => Reply(status, "{}"));

            var error = await Assert.ThrowsAsync<LlmException>(() => CreateClient(handler).SendAsync(Conversation));

            Assert.Equal(expectedTemporary, error.IsTemporary);
        }

        [Fact]
        public async Task SendAsync_puts_the_status_code_and_googles_message_in_the_error()
        {
            var handler = new FakeHandler(() => Reply(HttpStatusCode.Unauthorized, """{"error":{"message":"API key not valid"}}"""));

            var error = await Assert.ThrowsAsync<LlmException>(() => CreateClient(handler).SendAsync(Conversation));

            Assert.Contains("401", error.Message);
            Assert.Contains("API key not valid", error.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task SendAsync_fails_permanently_without_sending_when_the_key_is_missing(string apiKey)
        {
            var handler = new FakeHandler(() => Reply(HttpStatusCode.OK, OkJson));

            var error = await Assert.ThrowsAsync<LlmException>(() => CreateClient(handler, apiKey).SendAsync(Conversation));

            Assert.False(error.IsTemporary);
            Assert.Empty(handler.Requests);
        }

        private static Func<HttpResponseMessage> Sequence(params Func<HttpResponseMessage>[] steps)
        {
            var index = 0;
            return () => steps[Math.Min(index++, steps.Length - 1)]();
        }

        [Fact]
        public async Task SendAsync_retries_a_temporary_error_and_returns_the_later_success()
        {
            var handler = new FakeHandler(Sequence(
                () => Reply(HttpStatusCode.ServiceUnavailable, "{}"),
                () => Reply(HttpStatusCode.OK, OkJson)));

            var reply = await CreateClient(handler).SendAsync(Conversation);

            Assert.Equal("Hello", reply.Text);
            Assert.Equal(2, handler.Requests.Count);
        }

        [Fact]
        public async Task SendAsync_does_not_retry_a_permanent_error()
        {
            var handler = new FakeHandler(() => Reply(HttpStatusCode.Unauthorized, "{}"));

            await Assert.ThrowsAsync<LlmException>(() => CreateClient(handler).SendAsync(Conversation));

            Assert.Single(handler.Requests);
        }

        [Fact]
        public async Task SendAsync_gives_up_after_three_temporary_failures()
        {
            var handler = new FakeHandler(() => Reply(HttpStatusCode.ServiceUnavailable, "{}"));

            var error = await Assert.ThrowsAsync<LlmException>(() => CreateClient(handler).SendAsync(Conversation));

            Assert.True(error.IsTemporary);
            Assert.Equal(3, handler.Requests.Count);
        }

        [Fact]
        public async Task SendAsync_retries_when_the_network_fails()
        {
            var handler = new FakeHandler(Sequence(
                () => throw new HttpRequestException("no connection"),
                () => Reply(HttpStatusCode.OK, OkJson)));

            var reply = await CreateClient(handler).SendAsync(Conversation);

            Assert.Equal("Hello", reply.Text);
            Assert.Equal(2, handler.Requests.Count);
        }

        [Fact]
        public async Task SendAsync_treats_a_timeout_as_temporary_and_retries_it()
        {
            var handler = new FakeHandler(() => throw new TaskCanceledException("timeout"));

            var error = await Assert.ThrowsAsync<LlmException>(() => CreateClient(handler).SendAsync(Conversation));

            Assert.True(error.IsTemporary);
            Assert.Equal(3, handler.Requests.Count);
        }

        [Fact]
        public async Task SendAsync_does_not_retry_when_the_caller_cancels()
        {
            var handler = new FakeHandler(() => Reply(HttpStatusCode.OK, OkJson));
            using var cancelled = new CancellationTokenSource();
            cancelled.Cancel();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => CreateClient(handler).SendAsync(Conversation, cancelled.Token));

            Assert.True(handler.Requests.Count <= 1);
        }

        [Fact]
        public async Task SendAsync_rejects_an_empty_conversation_without_sending()
        {
            var handler = new FakeHandler(() => Reply(HttpStatusCode.OK, OkJson));

            await Assert.ThrowsAsync<ArgumentException>(() => CreateClient(handler).SendAsync(new List<LlmMessage>()));

            Assert.Empty(handler.Requests);
        }
    }
}
