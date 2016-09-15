using SaaSDemo.Web.Models;
using SaasKit.Multitenancy;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SaaSDemo.Web.Data;
using Microsoft.EntityFrameworkCore;
using SaaSDemo.Identity;

namespace SaaSDemo.Web.SaasKit
{
    public class AppTenantDatabaseResolver : ITenantResolver<AppTenant>
    {
        private readonly ApplicationDbContext _context;

        public AppTenantDatabaseResolver(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TenantContext<AppTenant>> ResolveAsync(HttpContext httpContext)
        {
            TenantContext<AppTenant> tenantContext = null;

            var subDomain = httpContext.Request.Host.Host.Split('.')[0].ToLower();
            var tenant = await _context.Tenants.FirstOrDefaultAsync(tn => tn.Subdomain == subDomain);
            if(tenant != null)
            {
                tenantContext = new TenantContext<AppTenant>(tenant);
            }
            else
            {
                //provide a default instance 
                tenantContext = new TenantContext<AppTenant>(new AppTenant {
                    AppTenantId = 666,
                    Name = "Recruitment Portal SaaS Demo",
                    Subdomain = "www" });
            }

            return tenantContext;
        }
    }
}
