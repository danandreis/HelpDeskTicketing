﻿using HelpDeskTicketing.Data.ViewModels;
using HelpDeskTicketing.Models;
using Microsoft.AspNetCore.Identity;

namespace HelpDeskTicketing.Data.Services
{
    public interface IUserService
    {
        Task<IEnumerable<AppUser>> GetUsers();
        Task<IEnumerable<AppUser>> GetUserAdmins(); //Get the list of systemAdmins to be selected when reasign ticket
        Task<AppUser> GetUserById(string Id); //Get user by Id
        Task<AppUser> GetUserByUserName(string userName); //Get user by UserName
        Task<AppUser> AddUser(UserVM userVM);
        Task<IdentityResult> UpdateUser(AppUser user);
        Task<bool> ResetPasswordByUser(string userName, string newPassword);
        Task<bool> ResetPasswordByAdmin(string Id);
        Task<AppUser> LoginUser(UserLoginVM userLoginVM);
        Task LogoutUser();
        Task<IEnumerable<Branch>> GetBranches();
        
    }
}
