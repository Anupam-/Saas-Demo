using SaaSDemo.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SaaSDemo.Identity
{
    public class ServicePlan
    {
        [Key]
        public int ServicePlanId { get; set; }
        [Required]
        public string Name { get; set; }
        
    }
}
