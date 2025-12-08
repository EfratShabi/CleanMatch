using AutoMapper;
using Clean.Core.DTO;
using Clean.Core.Entities;
using Clean.Core.Services;
using Clean.Service;
using CleanAPI.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CleanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbacksController : ControllerBase
    {
        private readonly IFeedbackService _feedbackServise;
        private readonly IMapper _mapper;

        public FeedbacksController(IFeedbackService feedbackServise, IMapper map)
        {
            _feedbackServise = feedbackServise;
            _mapper = map;

        }
        [HttpGet]
        public ActionResult Get()
        {
            var feedback = _feedbackServise.GetAllFeedbacks();
            return Ok(_mapper.Map<List<FeedbackDTO>>(feedback));
        }

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
        [HttpPost]
        public ActionResult Post([FromBody] FeedbackPostModel f)
        {
            var cl = _feedbackServise.GetAllFeedbacks().Find(s => s.CleanerId == f.CleanerId);
            var feedback = _mapper.Map<Feedback>(f);
            _feedbackServise.AddFeedback(feedback);
            return Ok(feedback);

        }

        //// PUT api/<FeedbacksController>/5
        [HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
            
        //}

        // DELETE api/<FeedbacksController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var f= _feedbackServise.GetFeedbackById(id);
            if (f != null)
            {
                _feedbackServise.DeleteFeedback(id);
            }

        }
    }
}
