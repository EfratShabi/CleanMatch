using Clean.Core.Agent;
using Clean.Core.Llm;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using System.Text.Json;

namespace CleanAPI.Agent
{
    // Talks to the MCP server (our own /mcp endpoint) on behalf of one customer.
    public class McpToolSession : IToolSession
    {
        private readonly McpClient _client;

        public McpToolSession(McpClient client)
        {
            _client = client;
        }

        public async Task<IReadOnlyList<LlmTool>> ListToolsAsync(CancellationToken ct = default)
        {
            var tools = await _client.ListToolsAsync(cancellationToken: ct);
            return tools.Select(tool => new LlmTool(tool.Name, tool.Description ?? "", tool.JsonSchema.GetRawText())).ToList();
        }

        public async Task<ToolResult> CallAsync(string name, string argumentsJson, CancellationToken ct = default)
        {
            var result = await _client.CallToolAsync(name, ParseArguments(argumentsJson), cancellationToken: ct);
            var text = string.Join("\n", result.Content.OfType<TextContentBlock>().Select(block => block.Text));
            return new ToolResult(text, result.IsError == true);
        }

        public ValueTask DisposeAsync() => _client.DisposeAsync();

        // The model wrote the arguments as JSON text; the MCP client wants a dictionary.
        public static Dictionary<string, object?> ParseArguments(string argumentsJson)
        {
            if (string.IsNullOrWhiteSpace(argumentsJson))
            {
                return new Dictionary<string, object?>();
            }

            return JsonSerializer.Deserialize<Dictionary<string, object?>>(argumentsJson) ?? new Dictionary<string, object?>();
        }
    }
}
