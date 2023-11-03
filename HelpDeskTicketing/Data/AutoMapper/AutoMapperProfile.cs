using AutoMapper;
using HelpDeskTicketing.Data.ViewModels;
using HelpDeskTicketing.Models;

namespace HelpDeskTicketing.Data.AutoMapper
{
    public class AutoMapperProfile:Profile
    {

        public AutoMapperProfile()
        {

            CreateMap<AppUser, UserVM>();
            CreateMap<UserVM, AppUser>();

        }
    }
}
