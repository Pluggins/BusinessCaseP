using BCP_Facial.Data;
using BCP_Facial.Models.ViewModels;
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
        private readonly ApplicationDbContext _db;

        public ClassController(
            ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            if (User.IsInRole("ADMIN"))
            {
                ClassViewModel model = new ClassViewModel();
                model.Classes = _db.Classes.Where(e => e.Deleted == false).OrderBy(e => e.Name).ToList();
                return View(model);
            } else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
