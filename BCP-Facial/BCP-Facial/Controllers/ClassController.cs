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

        [Route("Class/{id}")]
        public IActionResult Edit(string id)
        {
            ClassEditViewModel model = new ClassEditViewModel();
            Class thisClass = _db.Classes.Where(e => e.Deleted == false && e.Id.Equals(id)).FirstOrDefault();

            if (thisClass == null)
            {
                return RedirectToAction("Index", "Class");
            } else
            {
                List<Attendance> classAttendances = thisClass.List_Attendances.Where(e => e.Deleted == false).ToList();
                List <ClassEditAllocation> allocations = new List<ClassEditAllocation>();
                foreach (ClassAllocation item in thisClass.List_ClassAllocation.Where(e => e.Deleted == false))
                {
                    List<AttendanceItem> studentAttendances =  item.Student.List_AttendanceItems.Where(e => classAttendances.Contains(e.Attendance)).ToList();
                    ClassEditAllocation newAllocation = new ClassEditAllocation()
                    {
                        StudentId = item.Student.Id,
                        StudentName = item.Student.Name,
                        DateJoined = item.DateCreated,
                        AttendanceCount = studentAttendances.Count().ToString() + "/" + classAttendances.Count().ToString()
                    };

                    allocations.Add(newAllocation);
                }

                model.SelectedLecturer = thisClass.Lecturer;
                model.Lecturers = _db._BCPUsers.Where(e => e.Deleted == false).Where(e => e.Status == 2 || e.Status == 4).OrderBy(e => e.Name).ToList();
                model.ClassCode = thisClass.ClassCode.ToUpper();
                model.ClassId = thisClass.Id;
                model.ClassName = thisClass.Name;
                model.ClassAllocations = allocations;
                return View(model);
            }
            
        }
    }
}
