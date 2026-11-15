using AutoMapper;
using Clean.Core.DTO;
using Clean.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CleanAPI.Controllers
{
    [Route("api/cleaners/search")]
    [ApiController]
    [Authorize(Roles = "customer")]
    public class CleanerSearchController : ControllerBase
    {
        private readonly ICleanerSearchService _searchService;
        private readonly ICustomerService _customerService;
        private readonly IMapper _mapper;

        public CleanerSearchController(ICleanerSearchService searchService, ICustomerService customerService, IMapper mapper)
        {
            _searchService = searchService;
            _customerService = customerService;
            _mapper = mapper;
        }

        // GET api/cleaners/search?date=2026-11-23&start=09:00&hours=2&maxPrice=80
        // Distance is measured from the logged-in customer's address; maxPrice is a price per hour.
        [HttpGet]
        public async Task<ActionResult> Search([FromQuery, BindRequired] DateOnly date, [FromQuery, BindRequired] TimeOnly start,
            [FromQuery, BindRequired] double hours, [FromQuery] int? maxPrice)
        {
            var userId = User.GetUserId();
            var customer = userId == null ? null : await _customerService.GetCustomerByUserIdAsync(userId.Value);
            if (customer == null)
            {
                return Unauthorized();
            }

            try
            {
                var matches = await _searchService.SearchAsync(customer, date, start, hours, maxPrice);
                return Ok(_mapper.Map<List<CleanerSearchResultDTO>>(matches));
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
