using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IGames.Domain.DomainModels;
using IGames.Services.Interface;

namespace IGames.Web.Controllers
{
    [Authorize(Roles = Role.ADMIN)]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ImportUsers(IFormFile file)
        {

            //make a copy
            string filePath = $"{Directory.GetCurrentDirectory()}\\files\\{file.FileName}";

            using (FileStream fileStream = System.IO.File.Create(filePath))
            {
                file.CopyTo(fileStream);

                fileStream.Flush();
            }

            //read data from copy file
            List<string> errors = _userService.createUsersFromFile(filePath);

            if (errors.Count() == 0)
            {
                ViewData["Success"] = "Successfully imported all users!";
            }
            else
            {
                ViewData["Error"] = errors;
            }

            return View("Index");
        }

    }
}