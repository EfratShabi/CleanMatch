using Clean.Core.Agent;
using ModelContextProtocol.Client;

namespace CleanAPI.Agent
{
    // The address of the MCP server. It comes from configuration, never from the incoming request:
    // an address taken from the request's Host header would let a caller send the token to a server of their choice.
    public record McpSettings(string Endpoint);

    public class McpToolSessionFactory : IToolSessionFactory
    {
        private readonly McpSettings _settings;

        public McpToolSessionFactory(McpSettings settings)
        {
            _settings = settings;
        }

        public async Task<IToolSession> OpenAsync(string userToken, CancellationToken ct = default)
        {
            var transport = new HttpClientTransport(new HttpClientTransportOptions
            {
                Endpoint = new Uri(_settings.Endpoint),
                // The customer's own token: every tool then runs with her permissions.
                AdditionalHeaders = new Dictionary<string, string> { ["Authorization"] = "Bearer " + userToken }
            });

            var client = await McpClient.CreateAsync(transport, cancellationToken: ct);
            return new McpToolSession(client);
        }
    }
}
