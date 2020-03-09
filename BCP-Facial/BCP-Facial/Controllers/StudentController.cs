using BCP_Facial.Data;
using BCP_Facial.Models;
using BCP_Facial.Models.ViewModels;
using BCP_Facial.Services;
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

        [Route("Student/{id}")]
        public IActionResult GetUser(string id)
        {
            if (User.IsInRole("ADMIN"))
            {
                StudentViewModel model = new StudentViewModel();

                BCPUser user = _db._BCPUsers.Where(e => e.Id.Equals(id)).FirstOrDefault();
                if (user == null)
                {
                    return RedirectToAction("Index", "Student");
                } else
                {
                    model.StudentName = user.Name;
                    model.AccountRole = user.Status;
                    model.StudentImages = user.List_UserImage.Where(e => e.Deleted == false && e.Status == 2).OrderByDescending(e => e.Confidence).ToList();
                    model.StudentId = id;
                }

                ViewBag.SiteUrl = _db.SiteConfigs.Where(e => e.Key.Equals("SITEURL")).First().Value;
                return View(model);
            } else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Route("Student/Detail/{id}")]
        public IActionResult GetDetail(string id)
        {
            if (User.IsInRole("ADMIN"))
            {
                StudentViewModel model = new StudentViewModel();

                BCPUser user = _db._BCPUsers.Where(e => e.Id.Equals(id)).FirstOrDefault();
                if (user == null)
                {
                    return RedirectToAction("Index", "Student");
                } else
                {
                    model.StudentName = user.Name;
                    model.AccountRole = user.Status;
                    model.StudentImages = user.List_UserImage.Where(e => e.Deleted == false && e.Status == 2).OrderByDescending(e => e.Confidence).ToList();
                    model.StudentId = id;
                }

                ViewBag.SiteUrl = _db.SiteConfigs.Where(e => e.Key.Equals("SITEURL")).First().Value;
                return View(model);
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

        [Route("Student/AddFace/{id}")]
        public IActionResult AddFace(string id)
        {
            AspUserService aspUser = new AspUserService(_db, this);

            if (aspUser.IsAdmin)
            {
                BCPUser student = _db._BCPUsers.Where(e => e.Id.Equals(id)).FirstOrDefault();
                
                if (student == null)
                {
                    return RedirectToAction("Index", "Student");
                } else
                {
                    List<Recognizer> recognizers = _db.Recognizers.Where(e => e.Deleted == false).OrderBy(e => e.Id).ToList();
                    StudentViewModel studentModel = new StudentViewModel();
                    AddFaceViewModel model = new AddFaceViewModel();

                    studentModel.StudentName = student.Name;
                    studentModel.AccountRole = student.Status;
                    studentModel.StudentImages = student.List_UserImage.Where(e => e.Deleted == false && e.Status == 2).OrderByDescending(e => e.Confidence).ToList();
                    studentModel.StudentId = id;

                    ViewBag.SiteUrl = _db.SiteConfigs.Where(e => e.Key.Equals("SITEURL")).First().Value;
                    model.Student = studentModel;
                    model.Recognizers = recognizers;

                    return View(model);
                }
            } else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
