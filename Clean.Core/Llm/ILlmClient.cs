namespace Clean.Core.Llm
{
    public interface ILlmClient
    {
        // systemInstruction: standing rules for the model that are kept apart from the conversation itself.
        Task<LlmReply> SendAsync(
            IReadOnlyList<LlmMessage> conversation,
            IReadOnlyList<LlmTool>? tools = null,
            string? systemInstruction = null,
            CancellationToken ct = default);
    }
}
