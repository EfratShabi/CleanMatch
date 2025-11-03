using Clean.Core.Entities;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CleanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private IDataContext _context;
        public BookingsController(IDataContext context)
        {
            _context = context;
        }

        // GET: api/<BookingsController>
        [HttpGet]
        public ActionResult Get()
        {
            return Ok(_context.Bookings);
        }

        // GET api/<BookingsController>/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var b = _context.Bookings.Find(b => b.BookingCode == id);
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
            var exsitBooking = _context.Bookings.Find(b=> b.BookingCode==newBooking.BookingCode);
            if (exsitBooking == null)
            {
                _context.Bookings.Add(newBooking);
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
