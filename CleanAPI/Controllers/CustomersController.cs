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
        public ActionResult Post([FromBody] Customers customer)
        {
            var cl = _customerServise.GetCustomerByIdofCustomer(customer.IdOfCustomer);
            if (cl == null)
            {
                return Ok(_mapper.Map<Customers>(customer));
            }
            return Conflict();
        }

        // PUT api/<CustomersController>/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] CustomerPostModel cu)
        {
            var c = _customerServise.GetCustomerById(id);
            if (c == null)
            {
                return NotFound();
            }
            _mapper.Map(cu, c);
            _customerServise.UpdateCustomer(c);
            return Ok(_mapper.Map<CustomerDTO>(c));
        }

        // DELETE api/<CustomersController>/5
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var c = _customerServise.GetCustomerById(id);
            if (c == null)
            {
                return NotFound();
            }
            _customerServise.DeleteCustomer(id);
            return Ok();
        }
    }
}
