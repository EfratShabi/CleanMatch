using AutoMapper;
using Clean.Core.DTO;
using Clean.Core.Entities;
using Clean.Core.Services;
using Clean.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Clean.Core.Entities.User;

namespace CleanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // Roles are written per method: a method-level [Authorize(Roles)] ADDS to the class-level one (AND),
    // so a class-level "customer,manager" would lock cleaners out of the one endpoint they may use.
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IUserService _userService;
        private readonly ICleanerService _cleanerService;
        private readonly IBookingService _bookingService;
        private readonly IMapper _mapper;

        public CustomersController(ICustomerService customerService, IUserService userService,
            ICleanerService cleanerService, IBookingService bookingService, IMapper map)
        {
            _customerService = customerService;
            _userService = userService;
            _cleanerService = cleanerService;
            _bookingService = bookingService;
            _mapper = map;
        }

        // A customer may only touch their own profile; a manager may touch anyone's.
        // Customers.UserId is compared with the UserId inside the token.
        private bool CanAccess(Customers customer) =>
            User.IsManager() || customer.UserId == User.GetUserId();

        // A cleaner may see a customer only while she has an active (confirmed, not yet finished) booking with them.
        private async Task<bool> CleanerHasActiveBookingWithAsync(Customers customer)
        {
            var userId = User.GetUserId();
            if (userId == null || !User.IsInRole("cleaner")) return false;

            var cleaner = await _cleanerService.GetCleanerByUserIdAsync(userId.Value);
            return cleaner != null && await _bookingService.CleanerCanSeeCustomerAsync(cleaner.Id, customer.Id);
        }

        // GET: api/<CustomersController>
        // The full customers list contains personal details, so only a manager may see it.
        // (Planned for later: a cleaner may see a customer who has a booking with her.)
        [HttpGet]
        [Authorize(Roles = "manager")]
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
            if (!CanAccess(customer) && !await CleanerHasActiveBookingWithAsync(customer))
            {
                return Forbid();
            }
            return Ok(_mapper.Map<CustomerDTO>(customer));
        }

        // POST api/<CustomersController>
        [HttpPost]
        [AllowAnonymous] 
        public async Task<ActionResult> Post([FromBody] CustomerPostModel customerModel)
        {
            var existingCustomer = await _customerService.GetCustomerByIdOfCustomerAsync(customerModel.IdOfCustomer);

            if (existingCustomer != null)
            {
                return Conflict();
            }

            var newCustomer = _mapper.Map<Customers>(customerModel);
            try
            {
                await _customerService.ApplyLocationAsync(newCustomer);
            }
            catch (ArgumentException ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status400BadRequest);
            }

            var user = new User
            {
                UserName = customerModel.UserName,
                Password = customerModel.Password,
                Role = eRole.customer
            };
            var addedUser = await _userService.AddUserAsync(user);

            newCustomer.User = addedUser;
            newCustomer.UserId = addedUser.Id;

            await _customerService.AddCustomerAsync(newCustomer);

            return Ok(_mapper.Map<CustomerDTO>(newCustomer));
        }

        // PUT api/<CustomersController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "customer,manager")]
        public async Task<ActionResult> Put(int id, [FromBody] CustomerPutModel cu)
        {
            var c = await _customerService.GetCustomerByIdAsync(id);
            if (c == null)
            {
                return NotFound();
            }
            if (!CanAccess(c))
            {
                return Forbid();
            }

            var addressChanged = c.City != cu.City || c.Street != cu.Street || c.Mikud != cu.Mikud;
            _mapper.Map(cu, c);

            if (addressChanged)
            {
                try
                {
                    await _customerService.ApplyLocationAsync(c);
                }
                catch (ArgumentException ex)
                {
                    return Problem(ex.Message, statusCode: StatusCodes.Status400BadRequest);
                }
            }

            await _customerService.UpdateCustomerAsync(c);

            return Ok(_mapper.Map<CustomerDTO>(c));
        }

        // DELETE api/<CustomersController>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "customer,manager")]
        public async Task<ActionResult> Delete(int id)
        {
            var c = await _customerService.GetCustomerByIdAsync(id);
            if (c == null)
            {
                return NotFound();
            }
            if (!CanAccess(c))
            {
                return Forbid();
            }
            await _customerService.DeleteCustomerAsync(id);
            return Ok();
        }
    }
}