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
    public class UserRepository: IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<User> GetByUserNameAsync(string userName, string Password)
        {
            // מחפש משתמש שתואם לשם וסיסמה
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName && u.Password == Password);
        }

        public async Task<User> AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            // שימי לב: לא עושים כאן SaveChanges() לפי הוראת המורה!
            return user;
        }
    }
}
