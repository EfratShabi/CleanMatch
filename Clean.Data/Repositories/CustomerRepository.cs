using Clean.Core.Entities;
using Clean.Core.Repositories;
using Microsoft.EntityFrameworkCore; 
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

        public async Task<List<Customers>> GetAllCustomersAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<Customers?> GetCustomerByIdOfCustomerAsync(int IdOfCustomer)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.IdOfCustomer == IdOfCustomer);
        }

        public async Task<Customers?> GetCustomerByUserIdAsync(int userId)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Customers?> GetCustomerByIdAsync(int id)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddCustomerAsync(Customers newCustomer)
        {
            await _context.Customers.AddAsync(newCustomer);
        }

        public async Task UpdateCustomerAsync(Customers updatedCustomer)
        {

            _context.Customers.Update(updatedCustomer);

            await Task.CompletedTask;
        }

        public async Task DeleteCustomerAsync(int id)
        {
            var customer = await _context.Customers.Include(c => c.User).FirstOrDefaultAsync(c => c.Id == id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                _context.Users.Remove(customer.User);
            }
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}