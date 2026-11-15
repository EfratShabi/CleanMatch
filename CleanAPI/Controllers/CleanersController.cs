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
    // Every endpoint requires a logged-in user by default; the public ones (list, signup) opt out with [AllowAnonymous].
    // Note: a method-level [Authorize] ADDS to the class-level one (AND), so a role restriction written here
    // would block customers from the endpoints they need. That is why roles are written per method instead.
    [Authorize]
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

        // A cleaner may only touch her own profile; a manager may touch anyone's.
        // Cleaners.UserId is compared with the UserId inside the token.
        private bool CanAccess(Cleaners cleaner) =>
            User.IsManager() || cleaner.UserId == User.GetUserId();

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
        public async Task<ActionResult> Get(int id)
        {
            var a = await _cleanerService.GetCleanerByIdAsync(id);
            if (a == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CleanerDTO>(a));

        }




       

        //POST api/<CleanersController>
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Post([FromBody] CleanerPostModel c)
        {
            var cl = await _cleanerService.GetCleanerByIDOfCleaner(c.IdOfCleaner);
            if(cl!=null)
            {
                return Conflict();
            }

            var newCleaner = _mapper.Map<Cleaners>(c);
            try
            {
                await _cleanerService.ApplyLocationAsync(newCleaner);
            }
            catch (ArgumentException ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status400BadRequest);
            }

            var user=new User { UserName = c.UserName,Password=c.Password,Role=Clean.Core.Entities.User.eRole.cleaner };
            var User=await _userService.AddUserAsync(user);
            newCleaner.User = User;
            newCleaner.UserId = User.Id;

            await _cleanerService.AddCleanerAsync(newCleaner);
            return Ok();
        }

        // PUT api/<CleanersController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "cleaner,manager")]
        public async Task<ActionResult> Put(int id, [FromBody] CleanerPutModel cl)
        {
            var c = await _cleanerService.GetCleanerByIdAsync(id);
            if (c == null)
            {
                return NotFound();
            }
            if (!CanAccess(c))
            {
                return Forbid();
            }

            var addressChanged = c.City != cl.City || c.Street != cl.Street || c.Mikud != cl.Mikud;
            _mapper.Map(cl, c);

            if (addressChanged)
            {
                try
                {
                    await _cleanerService.ApplyLocationAsync(c);
                }
                catch (ArgumentException ex)
                {
                    return Problem(ex.Message, statusCode: StatusCodes.Status400BadRequest);
                }
            }

            await _cleanerService.UpdateCleanerAsync(c);

            return Ok(_mapper.Map<CleanerDTO>(c));
        }

        // DELETE api/<CleanersController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "cleaner,manager")]
        public async Task<ActionResult> Delete(int id)
        {
            var c = await _cleanerService.GetCleanerByIdAsync(id);
            if (c == null)
            {
                return NotFound();
            }
            if (!CanAccess(c))
            {
                return Forbid();
            }
            await _cleanerService.DeleteCleanerAsync(id);
            return Ok();
        }
    }
}
