using AutoMapper;
using Clean.Core.DTO;
using Clean.Core.Llm;
using Clean.Core.Models;
using Clean.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanAPI.Controllers
{
    [Route("api/cleaners/search/chat")]
    [ApiController]
    [Authorize(Roles = "customer")]
    public class SearchChatController : ControllerBase
    {
        private readonly ISearchConversationService _conversation;
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;
        private readonly ILogger<SearchChatController> _logger;

        public SearchChatController(ISearchConversationService conversation, ICustomerService customerService,
            IMapper mapper, ILogger<SearchChatController> logger)
        {
            _conversation = conversation;
            _customerService = customerService;
            _mapper = mapper;
            _logger = logger;
        }

        // POST api/cleaners/search/chat
        // The browser keeps the conversation: it sends the new text, what is already known, and the question it was just asked.
        [HttpPost]
        public async Task<ActionResult<SearchChatReplyDTO>> Post(SearchChatPostModel model, CancellationToken ct)
        {
            var userId = User.GetUserId();
            var customer = userId == null ? null : await _customerService.GetCustomerByUserIdAsync(userId.Value);
            if (customer == null)
            {
                return Unauthorized();
            }

            try
            {
                var result = await _conversation.HandleAsync(customer, model.Text, model.Known, model.Asked, ct);
                var results = result.Matches is null ? null : _mapper.Map<List<CleanerSearchResultDTO>>(result.Matches);
                return Ok(new SearchChatReplyDTO(result.Status, result.Intent, result.NextField, results));
            }
            catch (ArgumentException ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status400BadRequest);
            }
            catch (InvalidOperationException ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
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
        }
    }
}
