using Clean.Core.Entities;
using Clean.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks; // חובה

namespace CleanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService; // תיקון כתיב קטן (Service)

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        // GET: api/<ScheduleController>
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _scheduleService.GetAllAsync());
        }

        // GET api/<ScheduleController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            // שליפה ישירה ויעילה דרך הסרביס
            var s = await _scheduleService.GetByIdAsync(id);

            if (s == null)
            {
                return NotFound();
            }
            return Ok(s);
        }

        // POST api/<ScheduleController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Schedule newSchedule)
        {
            // הוספה אסינכרונית
            await _scheduleService.AddScheduleAsync(newSchedule);
            return Ok(newSchedule);
        }

        // PUT api/<ScheduleController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] Schedule updatedSchedule)
        {
            // בדיקה אם קיים
            var existingSchedule = await _scheduleService.GetByIdAsync(id);
            if (existingSchedule == null)
            {
                return NotFound();
            }

            await _scheduleService.UpdateScheduleAsync(updatedSchedule);
            return Ok(updatedSchedule);
        }

        // DELETE api/<ScheduleController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existingSchedule = await _scheduleService.GetByIdAsync(id);
            if (existingSchedule == null)
            {
                return NotFound();
            }

            await _scheduleService.DeleteScheduleAsync(id);
            return Ok();
        }
    }
}