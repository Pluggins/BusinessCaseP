using BCP_Facial.Data;
using BCP_Facial.Models;
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
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _db;

        public StudentController(
            ApplicationDbContext db)
        {
            _db = db;
        }

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

        public IActionResult Search(string category, string studentvalue)
        {
            if (User.IsInRole("ADMIN"))
            {
                StudentViewModel model = new StudentViewModel();
                if (string.IsNullOrEmpty(category) || string.IsNullOrEmpty(studentvalue))
                {
                    return RedirectToAction("Index", "Student");
                } else
                {
                    ViewBag.Value = studentvalue;
                    if (category.Equals("name"))
                    {
                        List<BCPUser> users = _db._BCPUsers.Where(e => e.Name.Contains(studentvalue) && e.Deleted == false && e.Status > 0).Take(10).ToList();
                        model.Students = users;
                        model.Category = category;
                        model.StudentValue = studentvalue;
                        return View(model);
                    }
                    else if (category.Equals("email"))
                    {
                        List<BCPUser> users = _db._BCPUsers.Where(e => e.Email.Contains(studentvalue) && e.Deleted == false && e.Status > 0).Take(10).ToList();
                        model.Students = users;
                        model.Category = category;
                        model.StudentValue = studentvalue;
                        return View(model);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Student");
                    }
                }
            } else
            {
                return RedirectToAction("Index", "Home");
            }
                
        }
    }
}
