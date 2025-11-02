using CleanAPI.Entities;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CleanAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private IDataContext _context;
        public CustomersController(IDataContext context)
        {
            _context = context;
        }


        // GET: api/<CustomersController>
        [HttpGet]
        public ActionResult Get()
        {
            return Ok(_context.Customers);

        }

        // GET api/<CustomersController>/5
        [HttpGet("{id}")]
        public ActionResult Get(int id)
        {
            var a = _context.Customers.Find(x => x.IdOfCustomer == id);
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
            var exsitCustomer=_context.Customers.Find(c=>c.IdOfCustomer==newCustomer.IdOfCustomer);
            if (exsitCustomer == null)
            {
                _context.Customers.Add(newCustomer);
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
