using Clean.Core.Entities;
using Clean.Core.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CleanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private ICustomerService _customerServise;
        public CustomersController(ICustomerService customerServise)
        {
            _customerServise = customerServise;
        }


        // GET: api/<CustomersController>
        [HttpGet]
        public ActionResult Get()
        {
            return Ok(_customerServise.GetAllCustomers());
        }

        // GET api/<CustomersController>/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var a = _customerServise.GetAllCustomers().Find(x => x.Id == id);
            if (a == null)
            {
                return NotFound();
            }
            return Ok(a);
        }

        // POST api/<CustomersController>
        [HttpPost]
        public ActionResult Post([FromBody] Customers newCustomer)
        {
            var exsitCustomer=_customerServise.GetAllCustomers().Find(c=>c.Id==newCustomer.Id);
            if (exsitCustomer == null)
            {
                _customerServise.GetAllCustomers().Add(newCustomer);
                return Ok(exsitCustomer);
            }
            else
                return Conflict(exsitCustomer);

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
