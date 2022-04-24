using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using IGames.Domain.DomainModels;
using IGames.Domain.Identity;
using IGames.Domain.Identity.DTO;
using IGames.Services.Interface;

namespace IGames.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserService _userService;
        private readonly List<SelectListItem> roles = new List<SelectListItem>
        {
             new SelectListItem  { Value = Role.ADMIN, Text = Role.ADMIN},
             new SelectListItem  { Value = Role.STANDARD_USER, Text = Role.STANDARD_USER}
        };
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, IUserService userService)
        {

            this._userManager = userManager;
            this._signInManager = signInManager;
            _userService = userService;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            UserRegistrationDto model = new UserRegistrationDto();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserRegistrationDto registerUser)
        {
            if (ModelState.IsValid)
            {
                var userExists = await _userManager.FindByEmailAsync(registerUser.Email);
                if (userExists == null)
                {
                    var user = new User
                    {
                        UserName = registerUser.Email,
                        NormalizedUserName = registerUser.Email,
                        Email = registerUser.Email,
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true,
                        UserCart = new ShoppingCart()
                    };

                    var result = await _userManager.CreateAsync(user, registerUser.Password);

                    if (result.Succeeded)
                    {
                        var resultRole = await _userManager.AddToRoleAsync(user, Role.STANDARD_USER);
                        if (resultRole.Succeeded)
                        {
                            return RedirectToAction("Login");
                        }
                        else
                        {
                            if (resultRole.Errors.Count() > 0)
                            {
                                foreach (var error in result.Errors)
                                {
                                    ModelState.AddModelError("message", error.Description);
                                }
                            }
                        }

                    }
                    else
                    {
                        if (result.Errors.Count() > 0)
                        {
                            foreach (var error in result.Errors)
                            {
                                ModelState.AddModelError("message", error.Description);
                            }
                        }
                        return View(registerUser);
                    }
                }
                else
                {
                    ModelState.AddModelError("message", "Email already exists.");
                    return View(registerUser);
                }
            }
            return View(registerUser);

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            UserLoginDto model = new UserLoginDto();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDto loginUser)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginUser.Email);
                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError("message", "Email not confirmed yet");
                    return View(loginUser);

                }
                if (await _userManager.CheckPasswordAsync(user, loginUser.Password) == false)
                {
                    ModelState.AddModelError("message", "Invalid credentials");
                    return View(loginUser);

                }

                var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, loginUser.RememberMe, true);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Role.STANDARD_USER.ToString());
                    return RedirectToAction("Index", "Home");
                }
                else if (result.IsLockedOut)
                {
                    return View("AccountLocked");
                }
                else
                {
                    ModelState.AddModelError("message", "Invalid login attempt");
                    return View(loginUser);
                }
            }
            return View(loginUser);
        }

        [HttpGet]
        [Authorize]
        public IActionResult ManageUsers()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var users = _userService.getAllUsers().Where(u => u.Id != userId).ToList();

            List<UserRolesDto> model = new List<UserRolesDto>();

            foreach (var user in users)
            {
                var roles = _userManager.GetRolesAsync(user).Result.ToList();
                model.Add(new UserRolesDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = roles
                });
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public IActionResult AddUserToRole(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            if (user != null)
            {
                var model = new UserAddOrRemoveRole
                {
                    Id = user.Id,
                    Email = user.Email,
                };
                ViewData["Roles"] = roles;
                return View(model);
            }

            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddUserToRole(UserAddOrRemoveRole addUserToRoleDto)
        {

            var user = _userManager.FindByIdAsync(addUserToRoleDto.Id).Result;

            var _model = new UserAddOrRemoveRole
            {
                Id = user.Id,
                Email = user.Email,
                RoleName = addUserToRoleDto.RoleName
            };

            ViewData["Roles"] = roles;

            if (user != null)
            {
                var roles = _userManager.GetRolesAsync(user).Result;
                if (roles.Contains(_model.RoleName))
                {
                    ViewData["Error"] = "User is already in role " + _model.RoleName;
                    return View(_model);
                }
                else
                {
                    var resultRole = _userManager.AddToRoleAsync(user, _model.RoleName).Result;
                    if (resultRole.Succeeded)
                    {
                        return RedirectToAction("ManageUsers");
                    }
                    else
                    {
                        ViewData["Error"] = "Could not add user to role";
                    }

                }

                return View(_model);
            }

            ViewData["Error"] = "User not found";
            return View(_model);

        }

        [HttpGet]
        [Authorize]
        public IActionResult RemoveUserFromRole(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            if (user != null)
            {
                var model = new UserAddOrRemoveRole
                {
                    Id = user.Id,
                    Email = user.Email,
                };

                ViewData["Roles"] = roles;
                return View(model);
            }

            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        [Authorize]
        public IActionResult RemoveUserFromRole(UserAddOrRemoveRole addUserToRoleDto)
        {

            var user = _userManager.FindByIdAsync(addUserToRoleDto.Id).Result;

            var _model = new UserAddOrRemoveRole
            {
                Id = user.Id,
                Email = user.Email,
                RoleName = addUserToRoleDto.RoleName
            };

            ViewData["Roles"] = roles;

            if (user != null)
            {
                var roles = _userManager.GetRolesAsync(user).Result;
                if (!roles.Contains(_model.RoleName))
                {
                    ViewData["Error"] = "User does not have role " + _model.RoleName;
                    return View(_model);
                }
                else
                {
                    var roleToRemove = new List<string>();
                    roleToRemove.Add(_model.RoleName);
                    var resultRole = _userManager.RemoveFromRolesAsync(user, roleToRemove.AsEnumerable()).Result;
                    if (resultRole.Succeeded)
                    {
                        return RedirectToAction("ManageUsers");
                    }
                    else
                    {
                        ViewData["Error"] = "Could not remove user from role";
                    }

                }

                return View(_model);
            }

            ViewData["Error"] = "User not found";
            return View(_model);

        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}
