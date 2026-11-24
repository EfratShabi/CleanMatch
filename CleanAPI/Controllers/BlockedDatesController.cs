using AutoMapper;
using Clean.Core.DTO;
using Clean.Core.Services;
using Clean.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanAPI.Controllers
{
    [Route("api/cleaners/{cleanerId}/blocked-dates")]
    [ApiController]
    [Authorize]
    public class BlockedDatesController : ControllerBase
    {
        private readonly IBlockedDateService _blockedDateService;
        private readonly ICleanerService _cleanerService;
        private readonly IMapper _mapper;

        public BlockedDatesController(IBlockedDateService blockedDateService, ICleanerService cleanerService, IMapper mapper)
        {
            _blockedDateService = blockedDateService;
            _cleanerService = cleanerService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> Get(int cleanerId)
        {
            if (await _cleanerService.GetCleanerByIdAsync(cleanerId) == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<List<BlockedDateDTO>>(await _blockedDateService.GetUpcomingAsync(cleanerId)));
        }

        [HttpPost]
        [Authorize(Roles = "cleaner,manager")]
        public async Task<ActionResult> Post(int cleanerId, [FromBody] BlockedDateRangeModel range)
        {
            var cleaner = await _cleanerService.GetCleanerByIdAsync(cleanerId);
            if (cleaner == null)
            {
                return NotFound();
            }
            if (!User.IsManagerOrOwner(cleaner.UserId))
            {
                return Forbid();
            }

            try
            {
                await _blockedDateService.BlockAsync(cleanerId, range.From, range.To);
                return Ok(_mapper.Map<List<BlockedDateDTO>>(await _blockedDateService.GetUpcomingAsync(cleanerId)));
            }
            catch (ArgumentException ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status400BadRequest);
            }
            catch (InvalidOperationException ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "cleaner,manager")]
        public async Task<ActionResult> Delete(int cleanerId, [FromQuery] DateOnly from, [FromQuery] DateOnly to)
        {
            var cleaner = await _cleanerService.GetCleanerByIdAsync(cleanerId);
            if (cleaner == null)
            {
                return NotFound();
            }
            if (!User.IsManagerOrOwner(cleaner.UserId))
            {
                return Forbid();
            }

            try
            {
                await _blockedDateService.UnblockAsync(cleanerId, from, to);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status400BadRequest);
            }
        }
    }
}
