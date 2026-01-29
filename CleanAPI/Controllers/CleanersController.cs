using AutoMapper;
using Clean.Core.DTO;
using Clean.Core.Entities;
using Clean.Core.Services;
using Clean.Service;
using CleanAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Clean.Core.Entities.User;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CleanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "cleaner,manager")]


    public class CleanersController : ControllerBase
    {
        //private IDataContext _cleanerServise;
        private IUserService _userService;
        private readonly ICleanerService _cleanerService;
        private readonly IMapper _mapper;

        public CleanersController(ICleanerService cleanerService,IUserService userService,IMapper map)
        {
            _cleanerService = cleanerService;
            _userService = userService;
            _mapper = map;
        }

        // GET: api/<CleanersController>
        [HttpGet]
        [AllowAnonymous]
        public  async Task<ActionResult> Get()
        {
            var cleaners=await _cleanerService.GetAllCleanersAsync();
            return Ok(_mapper.Map<List<CleanerDTO>>(cleaners));
        }

        //GET api/<CleanersController>/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var a =_cleanerService.GetCleanerByIdAsync(id);
            if (a == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CleanerDTO>(a));

        }




       

        //POST api/<CleanersController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CleanerPostModel c)
        {
            var user=new User { UserName = c.UserName,Password=c.Password,Role=Clean.Core.Entities.User.eRole.cleaner };
            var User=await _userService.AddUserAsync(user);
            var newCleaner = _mapper.Map<Cleaners>(c);
            newCleaner.User = User;
            newCleaner.UserId = User.Id;

            var cl = await _cleanerService.GetCleanerByIDOfCleaner(c.IdOfCleaner);
            if(cl!=null)
            {
                return Conflict(_mapper.Map<Cleaners>(c));
            }
            await _cleanerService.AddCleanerAsync(newCleaner);
            return Ok();
        }

        // PUT api/<CleanersController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] CleanerPutModel cl)
        {
            var c = await _cleanerService.GetCleanerByIdAsync(id);
            if (c == null)
            {
                return NotFound();
            }

            _mapper.Map(cl, c);

            await _cleanerService.UpdateCleanerAsync(c);

            return Ok(_mapper.Map<CleanerDTO>(c));
        }

        // DELETE api/<CleanersController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var c = _cleanerService.GetCleanerByIdAsync(id);
            if (c == null)
            {
                return NotFound();
            }
            _cleanerService.DeleteCleanerAsync(id);
            return Ok();
        }
    }
}
