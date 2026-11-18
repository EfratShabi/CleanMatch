using Clean.Core.Llm;
using System.Text.Json;

namespace Clean.Service
{
    public static class GeminiJson
    {
        public static string BuildRequest(IReadOnlyList<LlmMessage> conversation)
        {
            if (conversation.Count == 0)
            {
                throw new ArgumentException("The conversation is empty.");
            }

            var contents = conversation.Select(message => new
            {
                role = ToGeminiRole(message.Role),
                parts = new[] { new { text = message.Text } }
            });

            return JsonSerializer.Serialize(new { contents });
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

            root.TryGetProperty("usageMetadata", out var usage);
            return new LlmReply(
                text,
                ReadCount(usage, "promptTokenCount"),
                ReadCount(usage, "candidatesTokenCount") + ReadCount(usage, "thoughtsTokenCount"));
        }

        private static int ReadCount(JsonElement usage, string name) =>
            usage.ValueKind == JsonValueKind.Object && usage.TryGetProperty(name, out var value)
                ? value.GetInt32()
                : 0;

        private static string ToGeminiRole(string role) => role switch
        {
            "user" => "user",
            "assistant" => "model",
            _ => throw new ArgumentException($"Unknown role '{role}'.")
        };
    }
}
