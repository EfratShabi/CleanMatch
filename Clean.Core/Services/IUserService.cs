using Clean.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clean.Core.Services
{
    public interface IUserService
    {
        Task<User?> GetByUserNameAsync(string UserName, string Password);
        Task<User> AddUserAsync(User user);
    }
}
