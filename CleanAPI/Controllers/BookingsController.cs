using AutoMapper;
using Clean.Core.Entities;
using Clean.Core.Services;
using Clean.Service;
using CleanAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks; 

namespace CleanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IMapper _mapper;

        public BookingsController(IBookingService bookingService, IMapper map)
        {
            _bookingService = bookingService;
            _mapper = map;
        }

        // GET: api/<BookingsController>
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _bookingService.GetBookingsAsync());
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
            return Ok(b);
        }
        [HttpGet("by-cleaner/{cleanerId}")]
        public async Task<ActionResult> GetByCleaners(int cleanerId)
        {
            var b = await _bookingService.GetBookingByIDOfCleaners(cleanerId);
            if (b == null)
            {
                return NotFound();
            }
            return Ok(b);
        }

        // POST api/<BookingsController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] BookingPostModel bookingModel) // מקבלת את המודל הקצר
        {
            // 1. המרה מהמודל הקצר לישות המלאה של הדאטה בייס בעזרת המאפר
            var newBooking = _mapper.Map<Bookings>(bookingModel);

            try
            {
                // 2. שליחה לשירות (הסרוויס)
                await _bookingService.AddBookingAsync(newBooking);
                return Ok(newBooking);
            }
            catch (ArgumentException ex)
            {
                return Conflict(ex.Message);
            }
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] Bookings updatedBooking)
        {
            var existingBooking = await _bookingService.GetBookingsByIdAsync(id);

            if (existingBooking == null)
            {
                return NotFound();
            }

            await _bookingService.UpdateBookingAsync(updatedBooking);

            return Ok(updatedBooking);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existingBooking = await _bookingService.GetBookingsByIdAsync(id);

            if (existingBooking == null)
            {
                return NotFound();
            }

            await _bookingService.DeleteBookingAsync(id);

            return Ok();
        }
    }
}