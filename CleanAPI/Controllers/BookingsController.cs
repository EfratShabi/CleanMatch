using AutoMapper;
using Clean.Core.DTO;
using Clean.Core.Entities;
using Clean.Core.Services;
using Clean.Service;
using CleanAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ICustomerService _customerService;
        private readonly ICleanerService _cleanerService;
        private readonly IMapper _mapper;
        private readonly TimeProvider _timeProvider;

        public BookingsController(IBookingService bookingService, ICustomerService customerService,
            ICleanerService cleanerService, IMapper map, TimeProvider timeProvider)
        {
            _bookingService = bookingService;
            _customerService = customerService;
            _cleanerService = cleanerService;
            _mapper = map;
            _timeProvider = timeProvider;
        }

        // The "now" item lets AutoMapper compute IsCompleted (a booking that already ended).
        private BookingDTO ToDto(Bookings booking) =>
            _mapper.Map<BookingDTO>(booking, opt => opt.Items["now"] = IsraelTime.Now(_timeProvider));

        private List<BookingDTO> ToDtos(IEnumerable<Bookings> bookings) =>
            _mapper.Map<List<BookingDTO>>(bookings, opt => opt.Items["now"] = IsraelTime.Now(_timeProvider));

        // A booking points to PROFILES (CustomersId / CleanersId), while the token holds the USER id.
        // So we translate: token user -> customer/cleaner profile -> compare with the booking.
        private async Task<Customers?> GetMyCustomerAsync()
        {
            var userId = User.GetUserId();
            if (userId == null || !User.IsInRole("customer")) return null;
            return await _customerService.GetCustomerByUserIdAsync(userId.Value);
        }

        private async Task<Cleaners?> GetMyCleanerAsync()
        {
            var userId = User.GetUserId();
            if (userId == null || !User.IsInRole("cleaner")) return null;
            return await _cleanerService.GetCleanerByUserIdAsync(userId.Value);
        }

        // In which role is the logged-in user connected to this booking? null = not connected at all.
        private async Task<CancelledByRole?> GetRoleInBookingAsync(Bookings booking)
        {
            if (User.IsManager()) return CancelledByRole.Manager;

            var customer = await GetMyCustomerAsync();
            if (customer != null && customer.Id == booking.CustomersId) return CancelledByRole.Customer;

            var cleaner = await GetMyCleanerAsync();
            if (cleaner != null && cleaner.Id == booking.CleanersId) return CancelledByRole.Cleaner;

            return null;
        }

        // GET: api/<BookingsController>
        [HttpGet]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Get()
        {
            return Ok(ToDtos(await _bookingService.GetBookingsAsync()));
        }

        // The logged-in customer's or cleaner's own bookings, newest first, cancelled ones included.
        [HttpGet("mine")]
        [Authorize(Roles = "customer,cleaner")]
        public async Task<ActionResult> Mine()
        {
            var customer = await GetMyCustomerAsync();
            if (customer != null)
            {
                return Ok(ToDtos(await _bookingService.GetByCustomerAsync(customer.Id)));
            }

            var cleaner = await GetMyCleanerAsync();
            if (cleaner != null)
            {
                return Ok(ToDtos(await _bookingService.GetBookingByIDOfCleaners(cleaner.Id)));
            }

            return Unauthorized();
        }

        // GET api/<BookingsController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var b = await _bookingService.GetBookingsByIdAsync(id);
            if (b == null)
            {
                return NotFound();
            }
            if (await GetRoleInBookingAsync(b) == null)
            {
                return Forbid();
            }
            return Ok(ToDto(b));
        }

        // A cleaner may list her own bookings; a manager may list anyone's.
        [HttpGet("by-cleaner/{cleanerId}")]
        public async Task<ActionResult> GetByCleaners(int cleanerId)
        {
            if (!User.IsManager())
            {
                var me = await GetMyCleanerAsync();
                if (me == null || me.Id != cleanerId)
                {
                    return Forbid();
                }
            }
            var bookings = await _bookingService.GetBookingByIDOfCleaners(cleanerId);
            return Ok(ToDtos(bookings));
        }

        // POST api/<BookingsController>
        // Only a customer can book, and the booking is always made in HER name (taken from the token).
        [HttpPost]
        [Authorize(Roles = "customer")]
        public async Task<ActionResult> Post([FromBody] BookingPostModel bookingModel)
        {
            var customer = await GetMyCustomerAsync();
            if (customer == null)
            {
                return Unauthorized(); // valid token but the customer profile no longer exists
            }

            var newBooking = _mapper.Map<Bookings>(bookingModel);
            newBooking.CustomersId = customer.Id;

            try
            {
                await _bookingService.AddBookingAsync(newBooking);
                return Ok(ToDto(newBooking));
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

        // There is no PUT and no DELETE on purpose: a booking is cancelled (status Cancelled, kept for history) and a new one made.

        // Either side (the customer or the cleaner of the booking) may cancel, and so may a manager.
        [HttpPost("{id}/cancel")]
        public async Task<ActionResult> Cancel(int id)
        {
            var booking = await _bookingService.GetBookingsByIdAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            var role = await GetRoleInBookingAsync(booking);
            if (role == null)
            {
                return Forbid();
            }

            try
            {
                await _bookingService.CancelAsync(booking, role.Value);
                return Ok(ToDto(booking));
            }
            catch (InvalidOperationException ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status409Conflict);
            }
        }
    }
}
