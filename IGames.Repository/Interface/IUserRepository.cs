using IGames.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace IGames.Repository.Interface
{
        public interface IUserRepository
        {
            IEnumerable<User> GetAll();
            User Get(string id);
            void Insert(User entity);
            void Update(User entity);
            void Delete(User entity);
        }
 
}
