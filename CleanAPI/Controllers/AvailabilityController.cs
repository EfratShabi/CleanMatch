using AutoMapper;
using Clean.Core.DTO;
using Clean.Core.Entities;
using Clean.Core.Services;
using Clean.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanAPI.Controllers
{
    [Route("api/cleaners/{cleanerId}/availability")]
    [ApiController]
    [Authorize]
    public class AvailabilityController : ControllerBase
    {
        private readonly IAvailabilityService _availabilityService;
        private readonly ICleanerService _cleanerService;
        private readonly IMapper _mapper;

        public AvailabilityController(IAvailabilityService availabilityService, ICleanerService cleanerService, IMapper mapper)
        {
            _availabilityService = availabilityService;
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
            return Ok(_mapper.Map<List<AvailabilityDTO>>(await _availabilityService.GetByCleanerAsync(cleanerId)));
        }

        [HttpPut]
        [Authorize(Roles = "cleaner,manager")]
        public async Task<ActionResult> Put(int cleanerId, [FromBody] List<AvailabilitySlotModel> slots)
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
                await _availabilityService.ReplaceWeekAsync(cleanerId, _mapper.Map<List<Availability>>(slots));
                return Ok(_mapper.Map<List<AvailabilityDTO>>(await _availabilityService.GetByCleanerAsync(cleanerId)));
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
    }
}
