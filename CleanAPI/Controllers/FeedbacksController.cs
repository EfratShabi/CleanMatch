using AutoMapper;
using Clean.Core.DTO;
using Clean.Core.Entities;
using Clean.Core.Services;
using Clean.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // everything requires a logged-in user; POST and DELETE narrow it further by role below
    public class FeedbacksController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        private readonly ICustomerService _customerService;
        private readonly IBookingService _bookingService;
        private readonly IMapper _mapper;

        public FeedbacksController(IFeedbackService feedbackService, ICustomerService customerService,
            IBookingService bookingService, IMapper map)
        {
            _feedbackService = feedbackService;
            _customerService = customerService;
            _bookingService = bookingService;
            _mapper = map;
        }

        // GET: api/Feedbacks            all reviews, newest first
        // GET: api/Feedbacks?cleanerId=5   only this cleaner's reviews
        [HttpGet]
        public async Task<ActionResult> Get([FromQuery] int? cleanerId)
        {
            var feedbacks = cleanerId == null
                ? await _feedbackService.GetAllFeedbacksAsync()
                : await _feedbackService.GetFeedbacksByCleanerAsync(cleanerId.Value);
            return Ok(_mapper.Map<List<FeedbackDTO>>(feedbacks));
        }

        // GET api/Feedbacks/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var s = await _feedbackService.GetFeedbackByIdAsync(id);

            if (s == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<FeedbackDTO>(s));
        }

        // POST api/Feedbacks
        // Only the customer of a finished booking may review it (one review per booking).
        // The cleaner is taken from the booking, not from the request.
        [HttpPost]
        [Authorize(Roles = "customer")]
        public async Task<ActionResult> Post([FromBody] FeedbackPostModel f)
        {
            var userId = User.GetUserId();
            var customer = userId == null ? null : await _customerService.GetCustomerByUserIdAsync(userId.Value);
            if (customer == null)
            {
                return Unauthorized();
            }

            var booking = await _bookingService.GetBookingsByIdAsync(f.BookingCode);
            if (booking == null)
            {
                return Problem("ההזמנה לא נמצאה.", statusCode: StatusCodes.Status404NotFound);
            }
            if (booking.CustomersId != customer.Id)
            {
                return Forbid();
            }

            var feedback = _mapper.Map<Feedback>(f);

            try
            {
                await _feedbackService.AddFeedbackAsync(feedback, booking);
                return Ok(_mapper.Map<FeedbackDTO>(feedback));
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

        // DELETE api/Feedbacks/5
        // Manager only for now.
        [HttpDelete("{id}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Delete(int id)
        {
            var f = await _feedbackService.GetFeedbackByIdAsync(id);

            if (f != null)
            {
                await _feedbackService.DeleteFeedbackAsync(id);
                return Ok();
            }

            return NotFound();
        }
    }
}
