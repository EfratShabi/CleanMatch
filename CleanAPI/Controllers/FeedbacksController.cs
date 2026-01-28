using AutoMapper;
using Clean.Core.DTO;
using Clean.Core.Entities;
using Clean.Core.Services;
using CleanAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks; 

namespace CleanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbacksController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService; 
        private readonly IMapper _mapper;

        public FeedbacksController(IFeedbackService feedbackService, IMapper map)
        {
            _feedbackService = feedbackService;
            _mapper = map;
        }

        // GET: api/Feedbacks
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var feedbacks = await _feedbackService.GetAllFeedbacksAsync();
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
            return Ok(s);
        }

        // POST api/Feedbacks
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] FeedbackPostModel f)
        {
            // המרה
            var feedback = _mapper.Map<Feedback>(f);

            await _feedbackService.AddFeedbackAsync(feedback);

            return Ok(feedback);
        }

        // DELETE api/Feedbacks/5
        [HttpDelete("{id}")]
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