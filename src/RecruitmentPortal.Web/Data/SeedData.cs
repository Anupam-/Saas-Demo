using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecruitmentPortal.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
                    Email = "foobar3@outlook.com",
                    UserName = "foobar3",
                    EmailConfirmed = true,
                    NormalizedUserName = "FOOBAR3",
                    NormalizedEmail = "FOOBAR3@OUTLOOK.COM"
                };


                if (!context.Users.Any(u => u.Email == admin.Email))
                {
                    var userStore = new TenantEnabledUserStore(context, new AppTenant
                    {
                        AppTenantId = 1
                    });
                    await userStore.SetPasswordHashAsync(admin, new PasswordHasher<ApplicationUser>().HashPassword(admin, "Helpme123#"), default(CancellationToken));
                    await userStore.SetSecurityStampAsync(admin, Guid.NewGuid().ToString("D"), default(CancellationToken));
                    await userStore.CreateAsync(admin, default(CancellationToken));
                }
                
                context.SaveChanges();
            }
        }
    }
}
