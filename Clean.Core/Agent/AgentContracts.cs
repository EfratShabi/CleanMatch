using Clean.Core.Llm;

namespace Clean.Core.Agent
{
    // What a tool returned. IsError means the tool refused or failed; the text then explains why, so the model can react.
    public record ToolResult(string Text, bool IsError);

    // A connection that can list and run tools on behalf of ONE logged-in customer.
    public interface IToolSession : IAsyncDisposable
    {
        Task<IReadOnlyList<LlmTool>> ListToolsAsync(CancellationToken ct = default);

        Task<ToolResult> CallAsync(string name, string argumentsJson, CancellationToken ct = default);
    }

    public interface IToolSessionFactory
    {
        // userToken is the customer's own JWT, so every tool runs with her permissions and no more.
        Task<IToolSession> OpenAsync(string userToken, CancellationToken ct = default);
    }

    // The answer to show the customer, and every message this turn added (her message, tool requests and results,
    // the final answer). The browser sends those back next time, because the server keeps no memory.
    public record AgentTurn(string Text, IReadOnlyList<LlmMessage> NewMessages);

    public interface ISearchAgent
    {
        Task<AgentTurn> RunAsync(
            IReadOnlyList<LlmMessage> history, string userMessage, string userToken, CancellationToken ct = default);
    }
}
