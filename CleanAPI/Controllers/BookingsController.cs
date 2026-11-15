using AutoMapper;
using Clean.Core.DTO;
using Clean.Core.Entities;
using Clean.Core.Services;
using CleanAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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

        public BookingsController(IBookingService bookingService, ICustomerService customerService,
            ICleanerService cleanerService, IMapper map)
        {
            _bookingService = bookingService;
            _customerService = customerService;
            _cleanerService = cleanerService;
            _mapper = map;
        }

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

        // Allowed: a manager, the customer of the booking, or the cleaner of the booking.
        private async Task<bool> IsParticipantOrManagerAsync(Bookings booking)
        {
            if (User.IsManager()) return true;

            var customer = await GetMyCustomerAsync();
            if (customer != null && customer.Id == booking.CustomersId) return true;

            var cleaner = await GetMyCleanerAsync();
            return cleaner != null && cleaner.Id == booking.CleanersId;
        }

        // GET: api/<BookingsController>
        [HttpGet]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Get()
        {
            return Ok(_mapper.Map<List<BookingDTO>>(await _bookingService.GetBookingsAsync()));
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
            if (!await IsParticipantOrManagerAsync(b))
            {
                return Forbid();
            }
            return Ok(_mapper.Map<BookingDTO>(b));
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
            return Ok(_mapper.Map<List<BookingDTO>>(bookings));
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
                return Ok(_mapper.Map<BookingDTO>(newBooking));
            }
            catch (ArgumentException ex)
            {
                return Conflict(ex.Message);
            }
        }

        // There is no PUT on purpose: to change a booking, cancel it (DELETE) and book again.

        // DELETE api/<BookingsController>/5
        // Either side (the customer or the cleaner of the booking) may cancel, and so may a manager.
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existingBooking = await _bookingService.GetBookingsByIdAsync(id);

            if (existingBooking == null)
            {
                return NotFound();
            }
            if (!await IsParticipantOrManagerAsync(existingBooking))
            {
                return Forbid();
            }

            await _bookingService.DeleteBookingAsync(id);

            return Ok();
        }
    }
}
