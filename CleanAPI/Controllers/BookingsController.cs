using Clean.Core.Entities;
using Clean.Core.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CleanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private IBookingService _bookingService;
        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        // GET: api/<BookingsController>
        [HttpGet]
        public ActionResult Get()
        {
            return Ok(_bookingService.GetBookings());
        }

        // GET api/<BookingsController>/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var b = _bookingService.GetBookings().Find(b => b.BookingCode == id);
            if (b== null)
            {
                return NotFound();
            }
            return Ok(b);
        }

        // POST api/<BookingsController>
        [HttpPost]
        public ActionResult Post([FromBody] Bookings newBooking)
        {
            var exsitBooking = _bookingService.GetBookings().Find(b=> b.BookingCode==newBooking.BookingCode);
            if (exsitBooking == null)
            {
                _bookingService.GetBookings().Add(newBooking);
                return Ok(exsitBooking);
            }
            else
                return Conflict(exsitBooking);

        }

        // PUT api/<BookingsController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<BookingsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
