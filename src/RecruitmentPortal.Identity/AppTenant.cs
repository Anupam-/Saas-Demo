using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.Identity
{
    public class AppTenant
    {
        public int AppTenantId { get; set; }
        public string Name { get; set; }
        public string Subdomain { get; set; }
        public string Folder { get; set; }
    }
}
