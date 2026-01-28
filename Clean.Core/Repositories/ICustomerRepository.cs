using Clean.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; // <--- חובה להוסיף את זה!

namespace Clean.Core.Repositories
{
    public interface ICustomerRepository
    {
        Task<List<Customers>> GetAllCustomersAsync();
        Task<Customers?> GetCustomerByIdAsync(int id);
        Task<Customers?> GetCustomerByIdofCustomerAsync(int IdOfCustomer);
        Task AddCustomerAsync(Customers newCustomer);
        Task UpdateCustomerAsync(Customers newCustomer);
        Task DeleteCustomerAsync(int id);
        Task SaveAsync();
    }
}