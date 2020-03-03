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
    public class LecturerController : Controller
    {
        private readonly ApplicationDbContext _db;

        public LecturerController(
            ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            if (User.IsInRole("ADMIN"))
            {
                LecturerViewModel model = new LecturerViewModel();
                model.Lecturers = _db._BCPUsers.Where(e => e.Status == 2 || e.Status == 4).Where(e => e.Deleted == false).OrderBy(e => e.Name).ToList();
                return View(model);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
