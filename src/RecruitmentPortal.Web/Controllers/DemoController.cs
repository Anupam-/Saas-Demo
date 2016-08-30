using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RecruitmentPortal.Web.Controllers
{
    public class DemoController : Controller
    {
        [Authorize(Policy = "ProfServicePlan")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Policy = "BasicServicePlan")]
        public ActionResult BasicPlanAccess()
        {
            return View();
        }
    }


}