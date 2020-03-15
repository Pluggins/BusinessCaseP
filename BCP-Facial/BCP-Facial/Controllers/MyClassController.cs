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
    public class MyClassController : Controller
    {
        private readonly ApplicationDbContext _db;

        public MyClassController(
            ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize]
        public IActionResult Index()
        {
            if (User.IsInRole("LECTURER"))
            {
                MyClassViewModel model = new MyClassViewModel();
                List<MyClassItem> classItems = new List<MyClassItem>();
                AspUserService aspUser = new AspUserService(_db, this);

                if (aspUser.IsLecturer)
                {
                    List<Class> classes = aspUser.User.List_Classes.Where(e => e.Deleted == false).OrderByDescending(e => e.DateCreated).ToList();
                    foreach (Class item in classes)
                    {
                        MyClassItem newClassItem = new MyClassItem()
                        {
                            ClassName = item.Name,
                            ClassId = item.Id,
                            Capacity = item.List_ClassAllocation.Where(e => e.Deleted == false).Count().ToString() + "/" + item.Capacity.ToString(),
                            DateCreated = item.DateCreated
                        };

                        classItems.Add(newClassItem);
                    }

                    model.Classes = classItems;
                    return View(model);
                } else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Route("MyClass/{id}")]
        public IActionResult Detail(string id)
        {
            if (User.IsInRole("LECTURER"))
            {
                AspUserService aspUser = new AspUserService(_db, this);
                if (aspUser.IsLecturer)
                {
                    Class thisClass = aspUser.User.List_Classes.Where(e => e.Id.Equals(id) && e.Deleted == false).FirstOrDefault();
                    if (thisClass == null)
                    {
                        return RedirectToAction("Index", "Home");
                    } else
                    {
                        List<Attendance> classAttendances = thisClass.List_Attendances.Where(e => e.Deleted == false).ToList();
                        List<ClassAllocation> classAllocations = thisClass.List_ClassAllocation.Where(e => e.Deleted == false).ToList();
                        List<MyClassDetailItem> classDetails = new List<MyClassDetailItem>();

                        foreach (ClassAllocation item in classAllocations)
                        {
                            List<AttendanceItem> studentAttendances = item.Student.List_AttendanceItems.Where(e => classAttendances.Contains(e.Attendance)).ToList();
                            MyClassDetailItem newClassDetail = new MyClassDetailItem()
                            {
                                StudentId = item.Student.Id,
                                StudentName = item.Student.Name,
                                DateJoined = item.DateCreated,
                                AttendanceCount = studentAttendances.Count().ToString() + "/" + classAttendances.Count().ToString()
                            };

                            classDetails.Add(newClassDetail);
                        }

                        MyClassDetailViewModel model = new MyClassDetailViewModel()
                        {
                            ClassName = thisClass.Name,
                            ClassId = thisClass.Id,
                            Students = classDetails,
                            ClassCapacity = thisClass.Capacity.ToString(),
                            ClassCode = thisClass.ClassCode.ToUpper(),
                            LecturerName = thisClass.Lecturer.Name
                        };

                        return View(model);
                    }
                } else
                {
                    return RedirectToAction("Index", "Home");
                }
            } else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
