using Clean.Core.Entities;
using Clean.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Data.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly DataContext _context;
        public CustomerRepository(DataContext context) 
        {
            _context = context;
        }

        public List<Customers> GetAllCustomers()
        {
            return _context.Customers.ToList();
        }
        public Customers GetCustomerById(int id)
        {
            var customer = _context.Customers.ToList().Find(customer=>customer.Id == id);
            return customer;
        }




    }
}
