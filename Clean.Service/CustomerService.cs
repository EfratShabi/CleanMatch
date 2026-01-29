using Clean.Core.Entities;
using Clean.Core.Repositories;
using Clean.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Service
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<List<Customers>> GetAllCustomersAsync()
        {
            return await _customerRepository.GetAllCustomersAsync();
        }

        //public async Task<Customers?> GetCustomerByIdofCustomerAsync(int IdOfCustomer)
        //{
        //    return await _customerRepository.GetCustomerByIdofCustomerAsync(IdOfCustomer);
        //}

        public async Task<Customers?> GetCustomerByIdAsync(int id)
        {
            return await _customerRepository.GetCustomerByIdAsync(id);
        }

        public async Task AddCustomerAsync(Customers newCustomer)
        {
            await _customerRepository.AddCustomerAsync(newCustomer);
            await _customerRepository.SaveAsync();
        }

        public async Task UpdateCustomerAsync(Customers updatedCustomer)
        {
            await _customerRepository.UpdateCustomerAsync(updatedCustomer);
            await _customerRepository.SaveAsync();
        }

        public async Task DeleteCustomerAsync(int id)
        {
            await _customerRepository.DeleteCustomerAsync(id);
            await _customerRepository.SaveAsync();
        }
        public async Task SaveAsync()
        {
            await _customerRepository.SaveAsync();
        }
    }
}