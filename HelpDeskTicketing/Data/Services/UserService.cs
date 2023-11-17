using AutoMapper;
using HelpDeskTicketing.Data.ViewModels;
using HelpDeskTicketing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HelpDeskTicketing.Data.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IMapper _mapper;

        public UserService(AppDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, 
            SignInManager<AppUser> signInManager, IMapper mapper)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        public async Task<AppUser> AddUser(UserVM userVM)
        {

            if (userVM == null)
            {

                return null;

            }

            var user = await _userManager.FindByNameAsync(userVM.UserName);

            if(user == null) 
            {

                //If the username is not registered in DB the new user si created
                user = new AppUser
                {
                    EmailConfirmed = true,

                };

                userVM.Id = Guid.NewGuid().ToString();

                _mapper.Map(userVM, user);

                var branch = await _context.Branches.FirstOrDefaultAsync(b => b.Name == "IT Department");

                var role = (userVM.BranchId == branch.Id) ? "SystemAdmin" : "User";

                if (!await _roleManager.RoleExistsAsync(role))
                {

                    await _roleManager.CreateAsync(new IdentityRole(role));

                }

                var result = await _userManager.CreateAsync(user,userVM.Password);

                if(result.Succeeded)
                {

                    await _userManager.AddToRoleAsync(user, role);
                        
                }

            }
            else
            {

                //Set userID to null - it will be checked in the AccountCOntroller.
                //If userId = null - the the user already exists
                user.Id = null;

            }


            return user;

        }

        public async Task<IEnumerable<AppUser>> GetUsers()
        {

            return await _context.Users.Include(u=>u.Branch).ToListAsync();


        }

        public async Task<AppUser> LoginUser(UserLoginVM userLoginVM)
        {
            
            var userDB = await _userManager.FindByNameAsync(userLoginVM.UserName);

            if (userDB == null) {

                return null;

            }
            else
            {

                if(userDB.AccessFailedCount == 3)
                {

                    userLoginVM.isBlocked = true;
                    return null;

                }

                var passwordCheck = await _userManager.CheckPasswordAsync(userDB, userLoginVM.Password);

                if (!passwordCheck)
                {

                    if(userDB.AccessFailedCount < 3)
                    {

                        await _userManager.AccessFailedAsync(userDB);

                    }
                    
                    if(userDB.AccessFailedCount == 3)
                    {

                        userLoginVM.isBlocked = true;

                    }
                    
                    return null;

                }

                var result = await _signInManager.PasswordSignInAsync(userDB, userLoginVM.Password, false, false);

                if(!result.Succeeded)
                {

                    return null;
                }

                return userDB;
            }

        }

        public async Task<IdentityResult> UpdateUser(AppUser user)
        {
            
            return await _userManager.UpdateAsync(user);

        }

        public async Task<IEnumerable<Branch>> GetBranches()
        {

             return await _context.Branches.OrderBy(b=>b.Name).ToListAsync();

        }

        public async Task<AppUser> GetUser(string Id)
        {
            if (Id == null)
                return null;

            return await _userManager.FindByIdAsync(Id);

        }

        public async Task<bool> ResetPasswordByUser(string userName, string newPassword)
        {

            var userDB = await _userManager.FindByNameAsync(userName);

            if(userDB == null)
            {

                return false;

            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(userDB);

            IdentityResult identityResult = await _userManager.ResetPasswordAsync(userDB, token, newPassword);

            if(!identityResult.Succeeded)
            {

                return false;

            }

            return true;


        }

        public async Task<bool> ResetPasswordByAdmin(string Id)
        {

            var userDB = await _userManager.FindByIdAsync(Id);

            if(userDB == null)
            {

                return false;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(userDB);

            IdentityResult result = await _userManager.ResetPasswordAsync(userDB, token, "Password_1234");

            if(!result.Succeeded)
            {

                return false;

            }

            return true;

        }

        public async Task LogoutUser()
        {

            await _signInManager.SignOutAsync();

        }
    }
}
