using ExcelDataReader;
using IGames.Domain.Identity;
using IGames.Services.Interface;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using IGames.Repository.Interface;
using IGames.Domain.DomainModels;

namespace IGames.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepository;

        public UserService(UserManager<User> userManager, IUserRepository userRepository)
        {
            _userManager = userManager;
            _userRepository = userRepository;
        }

        public IEnumerable<User> getAllUsers()
        {
            return _userRepository.GetAll();
        }
        public List<string> createUsersFromFile(string filePath)
        {
            List<String> errorList = new List<string>();
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);


            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        string email = reader.GetValue(0).ToString();
                        string password = reader.GetValue(1).ToString();
                        string role = reader.GetValue(2).ToString();

                        var user = new User
                        {
                            Email = email,
                            UserName = email,
                            NormalizedUserName = email,
                            EmailConfirmed = true,
                            PhoneNumberConfirmed = true,
                            UserCart = new ShoppingCart()
                        };

                        var result = _userManager.FindByEmailAsync(user.Email).Result;
                        if (result == null)
                        {
                            var createUser = _userManager.CreateAsync(user, password).Result;
                            if (!createUser.Succeeded)
                            {
                                errorList.Add("Error creating user " + user.Email);
                            }
                            else
                            {
                                var res = _userManager.AddToRoleAsync(user, role.ToUpper()).Result;
                                if (!res.Succeeded)
                                {
                                    errorList.Add("Error addin user " + user.Email + " to role");
                                }
                            }
                        }
                        else
                        {
                            errorList.Add("User with email " + user.Email + " already exists!");
                        }
                    }
                }
            }

            return errorList;
        }

    }
}
