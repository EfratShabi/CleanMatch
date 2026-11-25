namespace Clean.Core.Llm
{
    // A tool the model may ask us to run. ParametersJsonSchema is a standard JSON Schema describing the arguments.
    public record LlmTool(string Name, string Description, string ParametersJsonSchema);

    // The model's request to run a tool. ProviderData is an opaque value the provider wants back unchanged
    // in the next request (for Gemini: a thought signature); only the provider's own client looks inside it.
    public record LlmToolCall(string Name, string ArgumentsJson, string? ProviderData = null);

    // Roles: "user", "assistant" (text, or a tool request when ToolCall is set), "tool" (a tool result: Text is the
    // result and ToolName says which tool produced it).
    public record LlmMessage(string Role, string Text, LlmToolCall? ToolCall = null, string? ToolName = null);

    public record LlmReply(string Text, int InputTokens, int OutputTokens, IReadOnlyList<LlmToolCall>? ToolCalls = null)
    {
        public bool HasToolCalls => ToolCalls is { Count: > 0 };
    }
}
