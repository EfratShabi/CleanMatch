using Clean.Core.Llm;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Clean.Service
{
    public static class GeminiJson
    {
        public static string BuildRequest(
            IReadOnlyList<LlmMessage> conversation, IReadOnlyList<LlmTool>? tools = null, string? systemInstruction = null)
        {
            if (conversation.Count == 0)
            {
                throw new ArgumentException("The conversation is empty.");
            }

            var request = new JsonObject { ["contents"] = ToContents(conversation) };

            if (!string.IsNullOrWhiteSpace(systemInstruction))
            {
                request["systemInstruction"] = new JsonObject
                {
                    ["parts"] = new JsonArray(new JsonObject { ["text"] = systemInstruction })
                };
            }

            if (tools is { Count: > 0 })
            {
                request["tools"] = new JsonArray(new JsonObject
                {
                    ["functionDeclarations"] = new JsonArray(tools.Select(ToDeclaration).ToArray())
                });
            }

            return request.ToJsonString();
        }

        public static LlmReply ParseReply(string json)
        {
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            if (!root.TryGetProperty("candidates", out var candidates) || candidates.GetArrayLength() == 0)
            {
                throw new LlmException("Gemini returned no answer (it may have been blocked).", isTemporary: false);
            }

            if (!candidates[0].TryGetProperty("content", out var content)
                || !content.TryGetProperty("parts", out var partsElement))
            {
                throw new LlmException("Gemini returned an empty answer.", isTemporary: false);
            }

            var text = string.Concat(partsElement.EnumerateArray()
                .Where(part => part.TryGetProperty("text", out _))
                .Select(part => part.GetProperty("text").GetString()));

            var toolCalls = new List<LlmToolCall>();
            foreach (var part in partsElement.EnumerateArray())
            {
                if (!part.TryGetProperty("functionCall", out var call))
                {
                    continue;
                }

                var name = call.GetProperty("name").GetString() ?? "";
                var arguments = call.TryGetProperty("args", out var args) ? args.GetRawText() : "{}";
                var signature = part.TryGetProperty("thoughtSignature", out var s) ? s.GetString() : null;
                toolCalls.Add(new LlmToolCall(name, arguments, signature));
            }

            root.TryGetProperty("usageMetadata", out var usage);
            return new LlmReply(
                text,
                ReadCount(usage, "promptTokenCount"),
                ReadCount(usage, "candidatesTokenCount") + ReadCount(usage, "thoughtsTokenCount"),
                toolCalls.Count > 0 ? toolCalls : null);
        }

        // Neighbouring messages with the same Google role become one content with several parts. Gemini needs this
        // when the model asks for several tools at once: all requests in one "model" turn, all results in one "user" turn.
        private static JsonArray ToContents(IReadOnlyList<LlmMessage> conversation)
        {
            var contents = new JsonArray();
            JsonArray? currentParts = null;
            string? currentRole = null;

            foreach (var message in conversation)
            {
                var (role, part) = ToPart(message);
                if (currentParts is null || role != currentRole)
                {
                    currentParts = new JsonArray();
                    currentRole = role;
                    contents.Add(new JsonObject { ["role"] = role, ["parts"] = currentParts });
                }
                currentParts.Add(part);
            }

            return contents;
        }

        private static (string Role, JsonObject Part) ToPart(LlmMessage message) => message.Role switch
        {
            "user" => ("user", new JsonObject { ["text"] = message.Text }),
            "assistant" when message.ToolCall is not null => ("model", ToFunctionCallPart(message.ToolCall)),
            "assistant" => ("model", new JsonObject { ["text"] = message.Text }),
            "tool" => ("user", ToFunctionResponsePart(message)),
            _ => throw new ArgumentException($"Unknown role '{message.Role}'.")
        };

        private static JsonObject ToFunctionCallPart(LlmToolCall call)
        {
            var part = new JsonObject
            {
                ["functionCall"] = new JsonObject
                {
                    ["name"] = call.Name,
                    ["args"] = ParseJson(call.ArgumentsJson, "{}")
                }
            };

            // Gemini wants its thought signature back exactly as it sent it.
            if (call.ProviderData is not null)
            {
                part["thoughtSignature"] = call.ProviderData;
            }

            return part;
        }

        private static JsonObject ToFunctionResponsePart(LlmMessage message)
        {
            if (string.IsNullOrWhiteSpace(message.ToolName))
            {
                throw new ArgumentException("A tool result needs the name of the tool.");
            }

            return new JsonObject
            {
                ["functionResponse"] = new JsonObject
                {
                    ["name"] = message.ToolName,
                    // Gemini requires the response to be a JSON object, so the result is wrapped.
                    ["response"] = new JsonObject { ["result"] = ParseJsonOrText(message.Text) }
                }
            };
        }

        private static JsonObject ToDeclaration(LlmTool tool) => new()
        {
            ["name"] = tool.Name,
            ["description"] = tool.Description,
            ["parametersJsonSchema"] = ParseJson(tool.ParametersJsonSchema, "{}")
        };

        private static JsonNode? ParseJson(string json, string fallbackWhenEmpty)
        {
            try
            {
                return JsonNode.Parse(string.IsNullOrWhiteSpace(json) ? fallbackWhenEmpty : json);
            }
            catch (JsonException error)
            {
                throw new ArgumentException("Not valid JSON: " + json, error);
            }
        }

        private static JsonNode? ParseJsonOrText(string text)
        {
            try
            {
                return JsonNode.Parse(text);
            }
            catch (JsonException)
            {
                return JsonValue.Create(text);
            }
        }

        private static int ReadCount(JsonElement usage, string name) =>
            usage.ValueKind == JsonValueKind.Object && usage.TryGetProperty(name, out var value)
                ? value.GetInt32()
                : 0;
    }
}
