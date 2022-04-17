using IGames.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace IGames.Services.Interface
{
    interface IUserService
    {
        List<string> createUsersFromFile(string filePath);
        public IEnumerable<User> getAllUsers();
    }
}
