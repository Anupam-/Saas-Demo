using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SaaSDemo.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SaaSDemo.Web.Data
{
    public static class SeedData
    {
        public static async void Initialize(IServiceProvider provider)
        {
            using (var context = new ApplicationDbContext(
                provider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                if(!context.ServicePlans.Any() && !context.Tenants.Any())
                {
                    var basicPlan = new ServicePlan {Name = "Basic"};
                    var proPlan = new ServicePlan {Name = "Professionell"};

                    var acmeTenant = new AppTenant
                    {
                        Name = "ACME",
                        Subdomain = "acme",
                        Folder = "acme"
                    };
                    var devTenant = new AppTenant
                    {
                        Name = "DevTenant 1",
                        Subdomain = "dev",
                        Folder = "dev"
                    };
                    acmeTenant.ServicePlan = basicPlan;
                    devTenant.ServicePlan = proPlan;

                    context.Tenants.AddRange(acmeTenant, devTenant);
                }
                await context.SaveChangesAsync();

                

                await context.SaveChangesAsync();
                var roleStore = new RoleStore<IdentityRole>(context);
                foreach (var role in new string[] { "Owner", "Administrator", "Manager", "Editor", "Applicant", "HR" })
                {
                    if (!roleStore.Roles.Any(r => r.Name == role))
                    {
                        await roleStore.CreateAsync(new IdentityRole(role));
                    }
                }

                await context.SaveChangesAsync();
                var admin = new ApplicationUser
                {
                    AppTenantId = 1,
                    Email = "foobar3@outlook.com",
                    UserName = "foobar3",
                    EmailConfirmed = true,
                    NormalizedUserName = "FOOBAR3",
                    NormalizedEmail = "FOOBAR3@OUTLOOK.COM"
                };

                /*
                 *we need to set up the USerStore raw, without the manager, because there's no request and therefore no AppTenant. We create a Mock instance and inject
                 * this one into the userstore, so we can create a testuser
                 */
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
