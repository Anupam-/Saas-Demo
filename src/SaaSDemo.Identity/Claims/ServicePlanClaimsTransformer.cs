using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using SaasKit.Multitenancy;
using Microsoft.EntityFrameworkCore;

namespace SaaSDemo.Identity.Claims
{
    public class ServicePlanClaimsTransformer : IClaimsTransformer
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsTransformationContext context)
        {
            //retrieve an instance of ApplicationDbContext
            var db = (ApplicationDbContext)context.Context.RequestServices.GetService(typeof(ApplicationDbContext));
            
            var identity = context.Principal.Identity;
            var principal = context.Principal;
            //get tenantContext from HttpContext
            var tenantContext = context.Context.GetTenantContext<AppTenant>();

            if(identity.IsAuthenticated)
            {
                //get Serviceplan from store; possible NullreferenceException.
                var servicePlan = db.Tenants.Include(x => x.ServicePlan).FirstOrDefault(t => t.AppTenantId == tenantContext.Tenant.AppTenantId).ServicePlan;
                //add new claim
                ((ClaimsIdentity)principal.Identity).AddClaim(new Claim("ServicePlan", servicePlan.Name));
            }
            return Task.FromResult(principal);
        }
    }
}
