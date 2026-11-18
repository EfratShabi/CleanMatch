namespace Clean.Core.Llm
{
    public interface ILlmClient
    {
        Task<LlmReply> SendAsync(IReadOnlyList<LlmMessage> conversation, CancellationToken ct = default);
    }
}
