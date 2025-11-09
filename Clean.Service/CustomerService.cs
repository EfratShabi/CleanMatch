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
    public class CustomerService:ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;
        public CustomerService (ICustomerRepository customerRepository)
        {
            _customerRepository= customerRepository;
        }

        public List<Customers> GetAllCustomers()
        {
            return _customerRepository.GetAllCustomers();
        }
        public Customers GetCustomerById(int id)
        {
            return _customerRepository.GetCustomerById(id);
        }

    }
}
