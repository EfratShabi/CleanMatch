using Clean.Core.Llm;
using Clean.Service;
using System.Text.Json;

namespace CleanerTest
{
    public class GeminiJsonTests
    {
        [Fact]
        public void BuildRequest_produces_the_json_shape_google_expects()
        {
            var conversation = new List<LlmMessage>
            {
                new("user", "Say hello"),
                new("assistant", "Hello!"),
                new("user", "And in French?")
            };

            var json = GeminiJson.BuildRequest(conversation);

            Assert.Equal(
                "{\"contents\":[" +
                "{\"role\":\"user\",\"parts\":[{\"text\":\"Say hello\"}]}," +
                "{\"role\":\"model\",\"parts\":[{\"text\":\"Hello!\"}]}," +
                "{\"role\":\"user\",\"parts\":[{\"text\":\"And in French?\"}]}" +
                "]}",
                json);
        }

        [Fact]
        public void BuildRequest_turns_our_assistant_role_into_googles_model_role()
        {
            var json = GeminiJson.BuildRequest(new List<LlmMessage> { new("assistant", "Hi") });

            using var doc = JsonDocument.Parse(json);
            Assert.Equal("model", doc.RootElement.GetProperty("contents")[0].GetProperty("role").GetString());
        }

        [Fact]
        public void BuildRequest_keeps_hebrew_text_intact()
        {
            var json = GeminiJson.BuildRequest(new List<LlmMessage> { new("user", "מנקה למחר בבוקר ברחובות") });

            using var doc = JsonDocument.Parse(json);
            var text = doc.RootElement.GetProperty("contents")[0].GetProperty("parts")[0].GetProperty("text").GetString();
            Assert.Equal("מנקה למחר בבוקר ברחובות", text);
        }

        [Fact]
        public void BuildRequest_rejects_an_empty_conversation()
        {
            Assert.Throws<ArgumentException>(() => GeminiJson.BuildRequest(new List<LlmMessage>()));
        }

        [Fact]
        public void BuildRequest_rejects_an_unknown_role()
        {
            Assert.Throws<ArgumentException>(() =>
                GeminiJson.BuildRequest(new List<LlmMessage> { new("robot", "Hi") }));
        }

        // The real answer Gemini returned for "Say hello in one short sentence." (signature shortened).
        private const string RealResponse = """
            {
              "candidates": [
                {
                  "content": {
                    "parts": [ { "text": "Hello, it is nice to meet you.", "thoughtSignature": "EpwBCpkBAWkU" } ],
                    "role": "model"
                  },
                  "finishReason": "STOP",
                  "index": 0
                }
              ],
              "usageMetadata": { "promptTokenCount": 8, "candidatesTokenCount": 9, "totalTokenCount": 17 },
              "modelVersion": "gemini-3.1-flash-lite"
            }
            """;

        [Fact]
        public void ParseReply_reads_text_and_token_counts_from_a_real_response()
        {
            var reply = GeminiJson.ParseReply(RealResponse);

            Assert.Equal("Hello, it is nice to meet you.", reply.Text);
            Assert.Equal(8, reply.InputTokens);
            Assert.Equal(9, reply.OutputTokens);
        }

        [Fact]
        public void ParseReply_joins_several_text_parts_and_ignores_parts_without_text()
        {
            var json = """
                { "candidates": [ { "content": { "parts": [
                    { "text": "Hello, " },
                    { "thoughtSignature": "abc" },
                    { "text": "world" }
                ] } } ] }
                """;

            Assert.Equal("Hello, world", GeminiJson.ParseReply(json).Text);
        }

        [Fact]
        public void ParseReply_adds_thinking_tokens_to_the_output_count()
        {
            var json = """
                { "candidates": [ { "content": { "parts": [ { "text": "ok" } ] } } ],
                  "usageMetadata": { "promptTokenCount": 10, "candidatesTokenCount": 5, "thoughtsTokenCount": 20 } }
                """;

            var reply = GeminiJson.ParseReply(json);

            Assert.Equal(10, reply.InputTokens);
            Assert.Equal(25, reply.OutputTokens);
        }

        [Fact]
        public void ParseReply_returns_zero_counts_when_usage_is_missing()
        {
            var json = """{ "candidates": [ { "content": { "parts": [ { "text": "ok" } ] } } ] }""";

            var reply = GeminiJson.ParseReply(json);

            Assert.Equal(0, reply.InputTokens);
            Assert.Equal(0, reply.OutputTokens);
        }

        [Fact]
        public void ParseReply_reads_hebrew_text_from_escaped_json()
        {
            var json = """{ "candidates": [ { "content": { "parts": [ { "text": "שלום" } ] } } ] }""";

            Assert.Equal("שלום", GeminiJson.ParseReply(json).Text);
        }

        [Theory]
        [InlineData("""{ "promptFeedback": { "blockReason": "SAFETY" } }""")]
        [InlineData("""{ "candidates": [] }""")]
        [InlineData("""{ "candidates": [ { "finishReason": "SAFETY" } ] }""")]
        public void ParseReply_throws_a_permanent_error_when_there_is_no_usable_answer(string json)
        {
            var error = Assert.Throws<LlmException>(() => GeminiJson.ParseReply(json));

            Assert.False(error.IsTemporary);
        }
    }
}
