namespace Clean.Core.Llm
{
    public interface ILlmClient
    {
        Task<LlmReply> SendAsync(
            IReadOnlyList<LlmMessage> conversation,
            IReadOnlyList<LlmTool>? tools = null,
            CancellationToken ct = default);
    }
}
