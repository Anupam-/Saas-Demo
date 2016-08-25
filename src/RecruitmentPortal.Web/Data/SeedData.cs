using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecruitmentPortal.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.Web.Data
{
    public static class SeedData
    {
        public async static void Initialize(IServiceProvider provider)
        {
            using (var context = new ApplicationDbContext(
                provider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if (!context.Tenants.Any())
                {
                    context.Tenants.AddRange(
                        new AppTenant
                        {
                            Name = "ACME",
                            Subdomain = "acme",
                            Folder = "acme"
                        },
                        new AppTenant
                        {
                            Name = "DevTenant 1",
                            Subdomain = "dev",
                            Folder = "dev"
                        });
                }

                var roleStore = new RoleStore<IdentityRole>(context);
                foreach (var role in new string[] { "Owner", "Administrator", "Manager", "Editor", "Applicant", "HR" })
                {
                    if (!roleStore.Roles.Any(r => r.Name == role))
                    {
                        await roleStore.CreateAsync(new IdentityRole(role));
                    }
                }
                context.SaveChanges();
                
                var admin = new ApplicationUser
                {
                    AppTenantId = 1,
                    Email = "marco.heinrich@outlook.com",
                    UserName = "heinrichm",
                    EmailConfirmed = true
                };

                
                if(!context.Users.Any(u => u.Email == admin.Email))
                {
                    var userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
                    await userManager.CreateAsync(admin, "Helpme123#");
                }
                
                context.SaveChanges();
            }
        }
    }
}
