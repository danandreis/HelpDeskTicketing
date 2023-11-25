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

namespace HelpDeskTicketing.Controllers
{
    public class AccountController : Controller
    {
        private readonly INotyfService _notyfService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;

        public AccountController(INotyfService notyfService, IUserService userService, IMapper mapper, UserManager<AppUser> userManager)
        {
            _notyfService = notyfService;
            _userService = userService;
            _mapper = mapper;
            _userManager = userManager;
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

            var userDB = await _userService.LoginUser(loginUser);

            if ( userDB != null)
            {

                _notyfService.Success("The user has been successfully authenticated!");

                if(_userManager.IsInRoleAsync(userDB,"SystemAdmin").Result)
                    return RedirectToAction("UsersList");

                if (_userManager.IsInRoleAsync(userDB,"User").Result)
                    return RedirectToAction("GetAllTickets", "UserTicket");

            }

            if (loginUser.isBlocked)
            {

                _notyfService.Error("Your account is blocked! Please contact thae administrator!");
                return View(loginUser);

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

            AppUser userDB = await _userService.GetUserById(Id);

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

            var userDB = await _userService.GetUserById(Id);

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

        [Authorize(Roles ="SystemAdmin")]
        public async Task<IActionResult> ChangeUserStatus(string Id)
        {

            var userDB = await _userService.GetUserById(Id);
            if (userDB != null)
            {

                userDB.AccessFailedCount = (userDB.AccessFailedCount != 3)? 3 : 0;

                await _userService.UpdateUser(userDB);

            }

            return RedirectToAction("UsersList");
        }

        [Authorize(Roles =("SystemAdmin"))]
        public async Task<IActionResult>ResetPasswordByAdmin(string Id)
        {

            if (!await _userService.ResetPasswordByAdmin(Id))
            {

                _notyfService.Error("An error occured when reseting password to user!");

            }

            _notyfService.Success("The password has been successfully reseted");

            return RedirectToAction("UsersList");
        }

        [Authorize]
        public async Task<IActionResult>ResetPasswordByUser()
        {

            ResetPasswordVM resetPasswordVM = new ResetPasswordVM();

            return View(resetPasswordVM);


        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordByUser(ResetPasswordVM resetPasswordVM)
        {

            if(!ModelState.IsValid)
            {

                return View(resetPasswordVM);
            }

            if (resetPasswordVM == null) 
            {

                _notyfService.Error("An error occured when reseting password!");
                return View(resetPasswordVM);
            
            }

            if(!await _userService.ResetPasswordByUser(User.Identity.Name,resetPasswordVM.Password))
            {

                _notyfService.Error("An error occured when reseting password!");
                return View(resetPasswordVM);

            }

            _notyfService.Success("The password has been successfully reseted!");
            return RedirectToAction("GetAllTickets","UserTicket");

        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {

            await _userService.LogoutUser();
            return RedirectToAction("Login");

        }

    }
}
