using HelpDeskTicketing.Models;
using Microsoft.AspNetCore.Identity;

namespace HelpDeskTicketing.Data
{
    public class AddUserAdmin
    {

        public static async Task AddAdmin(IApplicationBuilder app)
        {

            using(var serviceScope = app.ApplicationServices.CreateScope()) 
            {

                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if(!await roleManager.RoleExistsAsync("SystemAdmin"))
                {

                    await roleManager.CreateAsync(new IdentityRole("SystemAdmin"));

                }

                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var admin = await userManager.FindByEmailAsync("admin@mycompany.com");

                if(admin == null) 
                {

                    var adminUser = new AppUser()
                    {

                        FullName = "Popescu Ion",
                        UserName = "ion.popescu",
                        Email = "ion.popescu@mycompany.com",
                        PhoneNumber = "0725889912",
                        EmailConfirmed = true,
                        BranchId = "C9263569-330C-4647-A737-328D174AAF97"

                    };

                    await userManager.CreateAsync(adminUser, "Admin_1234");

                    await userManager.AddToRoleAsync(adminUser, "SystemAdmin");
                }     
            
            }

        }

    }
}
