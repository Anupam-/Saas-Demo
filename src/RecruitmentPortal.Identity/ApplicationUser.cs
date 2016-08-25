
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace RecruitmentPortal.Identity
{

    public class ApplicationUser : IdentityUser
    {
        public int AppTenantId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [NotMapped]
        public string FullName => FirstName + " " + LastName;

        [ForeignKey("AppTenantId")]
        public virtual AppTenant AppTenant { get; set; }
    }

}
