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
        private readonly ILocationService _locationService;

        public CustomerService(ICustomerRepository customerRepository, ILocationService locationService)
        {
            _customerRepository = customerRepository;
            _locationService = locationService;
        }

        public async Task ApplyLocationAsync(Customers customer)
        {
            var location = await _locationService.GetCoordinatesAsync(customer.City, customer.Street, customer.Mikud);
            if (location == null)
            {
                throw new ArgumentException("לא הצלחנו לאתר את הכתובת. בדקי עיר, רחוב ומיקוד ונסי שוב.");
            }
            customer.Latitude = location.Value.lat;
            customer.Longitude = location.Value.lon;
        }

        public async Task<List<Customers>> GetAllCustomersAsync()
        {
            return await _customerRepository.GetAllCustomersAsync();
        }

        public async Task<Customers?> GetCustomerByIdOfCustomerAsync(int IdOfCustomer)
        {
            return await _customerRepository.GetCustomerByIdOfCustomerAsync(IdOfCustomer);
        }

        public async Task<Customers?> GetCustomerByUserIdAsync(int userId)
        {
            return await _customerRepository.GetCustomerByUserIdAsync(userId);
        }

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