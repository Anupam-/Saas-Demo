using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace RecruitmentPortal.Identity.Claims
{
    public class ServicePlanClaimsTransformer : IClaimsTransformer
    {
        private AppTenant _appTenant;
        private ApplicationDbContext _context;
        private ClaimsPrincipal _principal;

        public ServicePlanClaimsTransformer(ClaimsPrincipal principal, ApplicationDbContext context, AppTenant tenant)
        {
            _principal = principal;
            _context = context;
            _appTenant = tenant;
        }

        public Task<ClaimsPrincipal> TransformAsync(ClaimsTransformationContext context)
        {
            if(_principal.Identity.IsAuthenticated)
            {
                //get from db
                var servicePlan = _context.Tenants.FirstOrDefault(t => t.AppTenantId == _appTenant.AppTenantId).ServicePlan;
                (_principal.Identity as ClaimsIdentity).AddClaim(new Claim("ServicePlan", servicePlan.Name));
            }
            return Task.FromResult(_principal);
        }
    }
}
