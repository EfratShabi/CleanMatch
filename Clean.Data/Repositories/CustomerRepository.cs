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
        public void AddCustomer(Customers newCustomer)
        {
            _context.Customers.Add(newCustomer);
        }
        public  void UpdateCustomer(Customers updatedCustomer)
        {
            _context.Customers.Update(updatedCustomer);
        }
        public void DeleteCustomer(int id)
        {
            _context.Customers.Remove(GetCustomerById(id));
        }
        public Customers GetCustomerByIdofCustomer(int IdOfCustomer)
            {
            var customer = _context.Customers.ToList().Find(customer => customer.IdOfCustomer == IdOfCustomer);
            return customer;
        }


        public void Save()
        {
            _context.SaveChanges();
        }




    }
}
