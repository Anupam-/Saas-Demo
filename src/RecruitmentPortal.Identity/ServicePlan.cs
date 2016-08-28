using RecruitmentPortal.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecruitmentPortal.Identity
{
    public class ServicePlan
    {
        public ServicePlan()
        {

        }
        [Key]
        public int ServicePlanId { get; set; }
        [Required]
        public string Name { get; set; }
        
    }
}
