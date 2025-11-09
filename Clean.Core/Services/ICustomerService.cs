using Clean.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Core.Services
{
   public interface ICustomerService
    {
        public List<Customers> GetAllCustomers();
        public Customers GetCustomerById(int id);
    }
}
