using AutoMapper;
using Clean.Core.DTO;
using Clean.Core.Entities;
using Clean.Core.Services;
using Clean.Service;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CleanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerServise;
        private readonly IMapper _mapper;


        public CustomersController(ICustomerService customerServise, IMapper map)
        {
            _customerServise = customerServise;
            _mapper = map;
        }


        // GET: api/<CustomersController>
        [HttpGet]
        public ActionResult Get()
        {
            var customers = _customerServise.GetAllCustomers();
            return Ok(_mapper.Map<List<CustomerDTO>>(customers));
        }

        // GET api/<CustomersController>/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var a = _customerServise.GetCustomerById(id);
            if (a == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CustomerDTO>(a));
        }

        // POST api/<CustomersController>
        [HttpPost]
        public ActionResult Post([FromBody] Customers newCustomer)
        {
            var cust=_mapper.Map<Customers>(newCustomer);
            var existingCustomer = _customerServise.GetCustomerByIdofCustomer(cust.IdOfCustomer);
            if (existingCustomer == null)
            {
                
                return Ok();
            }
            return Conflict();
            

        }

        // PUT api/<CustomersController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CustomersController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
