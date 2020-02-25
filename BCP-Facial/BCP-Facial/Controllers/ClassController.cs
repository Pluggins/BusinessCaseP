using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Controllers
{
    [Authorize]
    public class ClassController : Controller
    {
        public IActionResult Index()
        {
            if (User.IsInRole("ADMIN"))
            {
                return View();
            } else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
