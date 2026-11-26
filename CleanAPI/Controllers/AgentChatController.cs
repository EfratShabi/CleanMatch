using Clean.Core.Agent;
using Clean.Core.DTO;
using Clean.Core.Llm;
using Clean.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelContextProtocol;

namespace CleanAPI.Controllers
{
    [Route("api/agent/chat")]
    [ApiController]
    [Authorize(Roles = "customer")]
    public class AgentChatController : ControllerBase
    {
        private const string BearerPrefix = "Bearer ";

        private readonly ISearchAgent _agent;
        private readonly ILogger<AgentChatController> _logger;

        public AgentChatController(ISearchAgent agent, ILogger<AgentChatController> logger)
        {
            _agent = agent;
            _logger = logger;
        }

        // POST api/agent/chat
        // The browser sends the new message plus the history it received last time; the answer carries the new messages to send back.
        [HttpPost]
        public async Task<ActionResult<AgentChatReplyDTO>> Post(AgentChatPostModel model, CancellationToken ct)
        {
            var header = Request.Headers.Authorization.ToString();
            if (!header.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
            {
                return Unauthorized();
            }
            var token = header[BearerPrefix.Length..].Trim();

            try
            {
                var history = (model.History ?? new List<AgentMessageDTO>()).Select(ToMessage).ToList();
                var turn = await _agent.RunAsync(history, model.Text, token, ct);
                return Ok(new AgentChatReplyDTO(turn.Text, turn.NewMessages.Select(ToDto).ToList()));
            }
            catch (ArgumentException ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status400BadRequest);
            }
            catch (LlmException ex) when (ex.IsTemporary)
            {
                _logger.LogWarning(ex, "The language model is temporarily unavailable.");
                return Problem("השירות החכם עמוס כרגע, נסי שוב בעוד רגע.", statusCode: StatusCodes.Status503ServiceUnavailable);
            }
            catch (LlmException ex)
            {
                _logger.LogError(ex, "The language model failed with a permanent error.");
                return Problem("השירות החכם לא זמין כרגע.", statusCode: StatusCodes.Status502BadGateway);
            }
            catch (Exception ex) when (ex is McpException or HttpRequestException)
            {
                _logger.LogError(ex, "The tool server could not be reached.");
                return Problem("שירות הכלים לא זמין כרגע.", statusCode: StatusCodes.Status502BadGateway);
            }
        }

        private static LlmMessage ToMessage(AgentMessageDTO dto) => new(
            dto.Role,
            dto.Text,
            dto.ToolCall is null ? null : new LlmToolCall(dto.ToolCall.Name, dto.ToolCall.ArgumentsJson, dto.ToolCall.ProviderData),
            dto.ToolName);

        private static AgentMessageDTO ToDto(LlmMessage message) => new(
            message.Role,
            message.Text,
            message.ToolCall is null ? null : new AgentToolCallDTO(message.ToolCall.Name, message.ToolCall.ArgumentsJson, message.ToolCall.ProviderData),
            message.ToolName);
    }
}
