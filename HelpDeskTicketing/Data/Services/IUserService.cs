using HelpDeskTicketing.Data.ViewModels;
using HelpDeskTicketing.Models;
using Microsoft.AspNetCore.Identity;

namespace HelpDeskTicketing.Data.Services
{
    public interface IUserService
    {
        Task<IEnumerable<AppUser>> GetUsers();
        Task<AppUser> GetUser(string Id);
        Task<AppUser> AddUser(UserVM userVM);
        Task<IdentityResult> UpdateUser(AppUser user);
        Task<bool> ResetPasswordByUser(string userName, string newPassword);
        Task<bool> ResetPasswordByAdmin(string Id);
        Task<AppUser> LoginUser(UserLoginVM userLoginVM);
        Task LogoutUser();
        Task<IEnumerable<Branch>> GetBranches();
        
    }
}
