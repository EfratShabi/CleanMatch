using Clean.Core.Services;
using Clean.Service;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CleanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbacksController : ControllerBase
    {
        private IFeedbackService _feedbackServise;
        public FeedbacksController(IFeedbackService feedbackServise)
        {
            _feedbackServise = feedbackServise;
        }
        [HttpGet]
        public ActionResult Get()
        {
            return Ok(_feedbackServise.GetAllFeedbacks());
        }
        // GET: api/<FeedbacksController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<FeedbacksController>/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var s = _feedbackServise.GetAllFeedbacks().Find(s => s.Id == id);
            if (s == null)
            {
                return NotFound();
            }
            return Ok(s);
        }

        // POST api/<FeedbacksController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<FeedbacksController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<FeedbacksController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
