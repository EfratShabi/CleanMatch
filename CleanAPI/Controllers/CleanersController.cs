using AutoMapper;
using Clean.Core.DTO;
using Clean.Core.Entities;
using Clean.Core.Services;
using CleanAPI.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CleanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CleanersController : ControllerBase
    {
        //private IDataContext _cleanerServise;
        private readonly ICleanerService _cleanerServise;
        private readonly IMapper _mapper;

        public CleanersController(ICleanerService cleanerService,IMapper map)
        {
            _cleanerServise = cleanerService;
            _mapper = map;
        }

        // GET: api/<CleanersController>
        [HttpGet]
        public ActionResult Get()
        {
            var cleaners= _cleanerServise.GetAllCleaners();
            return Ok(_mapper.Map<List<CleanerDTO>>(cleaners));
        }

        //GET api/<CleanersController>/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var a =_cleanerServise.GetCleanerById(id);
            if (a == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CleanerDTO>(a));

        }

        //POST api/<CleanersController>
        [HttpPost]
        public ActionResult Post([FromBody] CleanerPostModel c)
        {
            var cl= _cleanerServise.GetCleanerByIDOfCleaner(c.IdOfCleaner);

            if(cl==null)
            {
                return Ok(_mapper.Map<Cleaners>(c));

            }
            return Conflict();
        }

        // PUT api/<CleanersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] CleanerPutModel cl)
        {
            var c = _cleanerServise.GetCleanerById(id);
            if (c == null)
            {
                NotFound();
            }
            Ok(_mapper.Map<CleanerDTO>(cl));

        }

        // DELETE api/<CleanersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var c = _cleanerServise.GetCleanerById(id);
            if (c == null)
            {
                NotFound();
            }
            _cleanerServise.DeleteCleaner(id);
            Ok();
        }
    }
}
