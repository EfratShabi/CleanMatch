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

        private static readonly LlmTool SearchTool = new(
            "search_cleaners",
            "Search for cleaners.",
            """{"type":"object","properties":{"hours":{"type":"number"}},"required":["hours"]}""");

        [Fact]
        public void BuildRequest_declares_the_tools_it_is_given()
        {
            var json = GeminiJson.BuildRequest(new List<LlmMessage> { new("user", "Hi") }, new[] { SearchTool });

            using var doc = JsonDocument.Parse(json);
            var declaration = doc.RootElement.GetProperty("tools")[0].GetProperty("functionDeclarations")[0];
            Assert.Equal("search_cleaners", declaration.GetProperty("name").GetString());
            Assert.Equal("Search for cleaners.", declaration.GetProperty("description").GetString());
            Assert.Equal("object", declaration.GetProperty("parametersJsonSchema").GetProperty("type").GetString());
        }

        [Fact]
        public void BuildRequest_has_no_tools_section_when_none_are_given()
        {
            using var doc = JsonDocument.Parse(GeminiJson.BuildRequest(new List<LlmMessage> { new("user", "Hi") }));

            Assert.False(doc.RootElement.TryGetProperty("tools", out _));
        }

        [Fact]
        public void BuildRequest_sends_a_tool_request_and_its_result_in_the_shape_google_expects()
        {
            var conversation = new List<LlmMessage>
            {
                new("user", "find one"),
                new("assistant", "", new LlmToolCall("search_cleaners", """{"hours":3}""", "SIG123")),
                new("tool", """[{"name":"Dana"}]""", ToolName: "search_cleaners")
            };

            using var doc = JsonDocument.Parse(GeminiJson.BuildRequest(conversation, new[] { SearchTool }));
            var contents = doc.RootElement.GetProperty("contents");

            Assert.Equal("model", contents[1].GetProperty("role").GetString());
            var callPart = contents[1].GetProperty("parts")[0];
            Assert.Equal("search_cleaners", callPart.GetProperty("functionCall").GetProperty("name").GetString());
            Assert.Equal(3, callPart.GetProperty("functionCall").GetProperty("args").GetProperty("hours").GetInt32());
            Assert.Equal("SIG123", callPart.GetProperty("thoughtSignature").GetString());

            Assert.Equal("user", contents[2].GetProperty("role").GetString());
            var response = contents[2].GetProperty("parts")[0].GetProperty("functionResponse");
            Assert.Equal("search_cleaners", response.GetProperty("name").GetString());
            Assert.Equal("Dana", response.GetProperty("response").GetProperty("result")[0].GetProperty("name").GetString());
        }

        [Fact]
        public void BuildRequest_wraps_a_plain_text_tool_result_as_a_string()
        {
            var conversation = new List<LlmMessage> { new("tool", "no cleaners found", ToolName: "search_cleaners") };

            using var doc = JsonDocument.Parse(GeminiJson.BuildRequest(conversation));
            var result = doc.RootElement.GetProperty("contents")[0].GetProperty("parts")[0]
                .GetProperty("functionResponse").GetProperty("response").GetProperty("result");

            Assert.Equal("no cleaners found", result.GetString());
        }

        [Fact]
        public void BuildRequest_rejects_a_tool_result_without_a_tool_name()
        {
            Assert.Throws<ArgumentException>(() =>
                GeminiJson.BuildRequest(new List<LlmMessage> { new("tool", "{}") }));
        }

        [Fact]
        public void BuildRequest_sends_the_system_instruction_apart_from_the_conversation()
        {
            var json = GeminiJson.BuildRequest(new List<LlmMessage> { new("user", "Hi") }, null, "Always answer in Hebrew.");

            using var doc = JsonDocument.Parse(json);
            Assert.Equal("Always answer in Hebrew.",
                doc.RootElement.GetProperty("systemInstruction").GetProperty("parts")[0].GetProperty("text").GetString());
            Assert.Equal(1, doc.RootElement.GetProperty("contents").GetArrayLength());
        }

        [Fact]
        public void BuildRequest_has_no_system_instruction_when_none_is_given()
        {
            using var doc = JsonDocument.Parse(GeminiJson.BuildRequest(new List<LlmMessage> { new("user", "Hi") }));

            Assert.False(doc.RootElement.TryGetProperty("systemInstruction", out _));
        }

        [Fact]
        public void BuildRequest_groups_several_tool_requests_and_their_results_into_one_turn_each()
        {
            var conversation = new List<LlmMessage>
            {
                new("user", "two things"),
                new("assistant", "", new LlmToolCall("a", "{}")),
                new("assistant", "", new LlmToolCall("b", "{}")),
                new("tool", "1", ToolName: "a"),
                new("tool", "2", ToolName: "b")
            };

            using var doc = JsonDocument.Parse(GeminiJson.BuildRequest(conversation));
            var contents = doc.RootElement.GetProperty("contents");

            Assert.Equal(3, contents.GetArrayLength());
            Assert.Equal(2, contents[1].GetProperty("parts").GetArrayLength());
            Assert.Equal("model", contents[1].GetProperty("role").GetString());
            Assert.Equal(2, contents[2].GetProperty("parts").GetArrayLength());
            Assert.Equal("user", contents[2].GetProperty("role").GetString());
        }

        [Fact]
        public void ParseReply_reads_a_tool_request_and_keeps_the_signature()
        {
            var json = """
                {
                  "candidates": [ { "content": { "role": "model", "parts": [
                    { "functionCall": { "name": "search_cleaners", "args": { "hours": 3, "date": "2026-11-25" } },
                      "thoughtSignature": "SIG123" }
                  ] } } ],
                  "usageMetadata": { "promptTokenCount": 40, "candidatesTokenCount": 9 }
                }
                """;

            var reply = GeminiJson.ParseReply(json);

            Assert.True(reply.HasToolCalls);
            var call = Assert.Single(reply.ToolCalls!);
            Assert.Equal("search_cleaners", call.Name);
            Assert.Equal("SIG123", call.ProviderData);
            using var args = JsonDocument.Parse(call.ArgumentsJson);
            Assert.Equal(3, args.RootElement.GetProperty("hours").GetInt32());
            Assert.Equal("", reply.Text);
        }

        [Fact]
        public void ParseReply_has_no_tool_requests_for_a_plain_text_answer()
        {
            var reply = GeminiJson.ParseReply("""{"candidates":[{"content":{"parts":[{"text":"Hi"}]}}]}""");

            Assert.False(reply.HasToolCalls);
            Assert.Null(reply.ToolCalls);
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
