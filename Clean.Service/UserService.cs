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
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // Fetch the user by name only, then compare the typed password to the stored hash.
        // The comparison must happen in code: BCrypt creates a different hash each time (random salt),
        // so the database can't compare passwords for us.
        public async Task<User?> GetByUserNameAsync(string userName, string password)
        {
            var user = await _userRepository.GetByUserNameAsync(userName);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return null;
            }
            return user;
        }

        // Hash the password before it is saved - the plain text never reaches the database.
        public async Task<User> AddUserAsync(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            return await _userRepository.AddUserAsync(user);
        }

    }
}
