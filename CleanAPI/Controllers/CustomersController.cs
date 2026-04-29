using AutoMapper;
using Clean.Core.DTO;
using Clean.Core.Entities;
using Clean.Core.Services;
using CleanAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Clean.Core.Entities.User;

namespace CleanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "customer,manager")]


    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public CustomersController(ICustomerService customerService, IUserService userService, IMapper map)
        {
            _customerService = customerService;
            _userService = userService;
            _mapper = map;
        }

        // GET: api/<CustomersController>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> Get()
        {
            var customers = await _customerService.GetAllCustomersAsync();
            return Ok(_mapper.Map<List<CustomerDTO>>(customers));
        }

        // GET api/<CustomersController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var customer = await _customerService.GetCustomerByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CustomerDTO>(customer));
        }

        // POST api/<CustomersController>
        [HttpPost]
        [AllowAnonymous] 
        public async Task<ActionResult> Post([FromBody] CustomerPostModel customerModel)
        {
            var existingCustomer = await _customerService.GetCustomerByIdAsync(customerModel.IdOfCustomer);

            if (existingCustomer != null)
            {
                return Conflict();
            }

            var user = new User
            {
                UserName = customerModel.UserName,
                Password = customerModel.Password,
                Role = eRole.customer 
            };
            var addedUser = await _userService.AddUserAsync(user);

            var newCustomer = _mapper.Map<Customers>(customerModel);

            newCustomer.User = addedUser;
            newCustomer.UserId = addedUser.Id;

            await _customerService.AddCustomerAsync(newCustomer);

            return Ok(_mapper.Map<CustomerDTO>(newCustomer));
        }

        // PUT api/<CustomersController>/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromBody] CustomerPostModel cu)
        {
            var c = await _customerService.GetCustomerByIdAsync(id);
            if (c == null)
            {
                return NotFound();
            }

            _mapper.Map(cu, c);

            await _customerService.UpdateCustomerAsync(c);

            return Ok(_mapper.Map<CustomerDTO>(c));
        }

        // DELETE api/<CustomersController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var c = await _customerService.GetCustomerByIdAsync(id);
            if (c == null)
            {
                return NotFound();
            }
            await _customerService.DeleteCustomerAsync(id);
            return Ok();
        }
    }
}