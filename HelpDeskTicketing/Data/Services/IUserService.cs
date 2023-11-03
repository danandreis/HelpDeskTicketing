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
        Task<AppUser> ResetPassword();
        Task<bool> LoginUser(UserLoginVM userLoginVM);
        Task<bool> BlockUser();
        Task<IEnumerable<Branch>> GetBranches();
        
    }
}
