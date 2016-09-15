using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SaaSDemo.Identity
{
    public class AppTenant
    {
        public int AppTenantId { get; set; }
        public string Name { get; set; }
        public string Subdomain { get; set; }
        public string Folder { get; set; }

        public int ServicePlanId { get; set; }

        [ForeignKey("ServicePlanId")]
        public virtual ServicePlan ServicePlan { get; set; }
    }

}
