using CleanAPI.Entities;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CleanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CleanersController : ControllerBase
    {
        private IDataContext _context;
        public CleanersController(IDataContext context)
        {
            _context = context;
        }

        // GET: api/<CleanersController>
        [HttpGet]
        public ActionResult Get()
        {
            return Ok(_context.Cleaners);
        }

        // GET api/<CleanersController>/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var a = _context.Cleaners.Find(x => x.IdOfCleaner == id);
            if (a == null) 
            {
                return NotFound();
            }
            return Ok(a);

        }

        // POST api/<CleanersController>
        [HttpPost]
        public ActionResult Post([FromBody] Cleaners newCleaner)
        {
            var existCleaner = _context.Cleaners.Find(c => c.IdOfCleaner == newCleaner.IdOfCleaner);
            if (existCleaner == null) 
            {
                 _context.Cleaners.Add(newCleaner);
                return Ok(existCleaner);
            }
            else
                return Conflict(existCleaner);
        }

        // PUT api/<CleanersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CleanersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
