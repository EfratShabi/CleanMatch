using Clean.Core.Entities;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CleanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private IDataContext _context;
        public ScheduleController(IDataContext context)
        {
            _context = context;
        }

        // GET: api/<ScheduleController>
        [HttpGet]
        public ActionResult Get()
        {
            return Ok(_context.Schedule);
        }

        // GET api/<ScheduleController>/5
        //[HttpGet("{id}")]
        //public ActionResult Get(int id)
        //{
        //    //var s = _context.Schedule.Find(s => s.CodeCleaner== id);
        //    //if (s== null)
        //    //{
        //    //    return NotFound();
        //    //}
        //    //return Ok(s);
        //}

        // POST api/<ScheduleController>
        //[HttpPost]
        //public ActionResult Post([FromBody] Schedule newSchedule)
        //{
           
        //}

        // PUT api/<ScheduleController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ScheduleController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
