
using Clean.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Core.Repositories
{
    public interface ICustomerRepository
    {
        public List<Customers> GetAllCustomers();
        public  Customers GetCustomerById(int id);
    }
}
