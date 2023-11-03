using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using HelpDeskTicketing.Data.Services;
using HelpDeskTicketing.Data.ViewModels;
using HelpDeskTicketing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace HelpDeskTicketing.Controllers
{
    public class AccountController : Controller
    {
        private readonly INotyfService _notyfService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public AccountController(INotyfService notyfService, IUserService userService, IMapper mapper)
        {
            _notyfService = notyfService;
            _userService = userService;
            _mapper = mapper;
        }

        public IActionResult Login()
        {

            var loginUser = new UserLoginVM();
            return View(loginUser);

        }

        [HttpPost]
        public async Task<IActionResult>Login(UserLoginVM loginUser)
        {

            if (!ModelState.IsValid)
            {

                return View(loginUser);
            }

            if (await _userService.LoginUser(loginUser))
            {

                _notyfService.Success("The user has been successfully authenticated!");

                return RedirectToAction("UsersList");


            }
            
            _notyfService.Error("The login credentials are incorrect!");

            return View(loginUser);

        }

        public IActionResult AccessDenied(string ReturnURL)
        {

            return View();

        }

        [Authorize(Roles ="SystemAdmin")]
        public async Task<IActionResult> UsersList()
        {

            var users = await _userService.GetUsers();
            return View(users);

        }

        public async Task<IActionResult>NewUser()
        {

            var newUser = new UserVM();
            ViewBag.Branches = new SelectList(await _userService.GetBranches(), "Id", "Name");

            return View(newUser);
        }

        [HttpPost]
        public async Task<IActionResult>NewUser(UserVM userVM)
        {

            if(!ModelState.IsValid)
            {

                ViewBag.Branches = new SelectList(await _userService.GetBranches(), "Id", "Name");

                return View(userVM);

            }

            AppUser newUser = await _userService.AddUser(userVM);

            if (newUser.Id == null)
            {

                _notyfService.Error("There is already this username registered in Database!");

                return RedirectToAction("NewUser");

            }
          
            if (newUser == null)
            {

                _notyfService.Error("There was an error when adding the new user to the database!");

                return RedirectToAction("UsersList");


            }


            _notyfService.Success("The user has been successfully added to database!");

            return RedirectToAction("UsersList");

        }

        [Authorize(Roles ="SystemAdmin")]
        public async Task<IActionResult>Edit(string Id)
        {

            if (Id==null)
            {

                _notyfService.Error("The id is invalid!");

                return RedirectToAction("UsersList");
            }

            AppUser userDB = await _userService.GetUser(Id);

            if (userDB == null) 
            {

                _notyfService.Error("There is no user with this ID in the database!");

                return RedirectToAction("UsersList");

            }

            ViewBag.Branches = new SelectList(await _userService.GetBranches(),"Id","Name");

            var user = new UserVM();
            _mapper.Map(userDB, user);

            return View(user);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(string Id,UserVM userMV)
        {

            ModelState["Password"].ValidationState = ModelValidationState.Valid;
            ModelState["ConfirmPassword"].ValidationState = ModelValidationState.Valid;

            if (!ModelState.IsValid)
            {

                ViewBag.Branches = new SelectList(await _userService.GetBranches(), "Id", "Name");

                return View(userMV);

            }

            var userDB = await _userService.GetUser(Id);

            _mapper.Map(userMV, userDB);

            IdentityResult result = await _userService.UpdateUser(userDB);

            if (!result.Succeeded)
            {

                _notyfService.Success("An error occured when updating user data!");
                return RedirectToAction("usersList");

            }

            _notyfService.Success("The user data has been successfully updated!");
            return RedirectToAction("UsersList");

        }

    }
}
