using SaaSDemo.Web.Models;
using SaasKit.Multitenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SaaSDemo.Identity;

namespace SaaSDemo.Web.SaasKit
{
    public class AppTenantResolver : ITenantResolver<AppTenant>
    {
        IEnumerable<AppTenant> _tenants = new List<AppTenant> {
            new AppTenant {
                Name = "ACME",
                Subdomain = "ACME"
            }
        };

        public async Task<TenantContext<AppTenant>> ResolveAsync(HttpContext context)
        {
            TenantContext<AppTenant> tenantContext = null;

            var subDomain = context.Request.Host.Host.Split('.')[0].ToLower();
            var tenant = _tenants.FirstOrDefault(tn => tn.Subdomain == subDomain);
            if (tenant != null)
            {
                tenantContext = new TenantContext<AppTenant>(tenant);
            }
            else
            {
                //provide a default instance 
                tenantContext = new TenantContext<AppTenant>(new AppTenant { Name = "Recruitment Portal SaaS Demo", Subdomain = "www" });
            }

            return await Task.FromResult(tenantContext);
        }
    }
}
