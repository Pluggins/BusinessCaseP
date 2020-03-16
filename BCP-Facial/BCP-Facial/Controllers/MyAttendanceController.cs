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
    public class MyAttendanceController : Controller
    {
        private readonly ApplicationDbContext _db;

        public MyAttendanceController(
            ApplicationDbContext db)
        {
            _db = db;
        }

        [Authorize]
        public IActionResult Index()
        {
            AspUserService aspUser = new AspUserService(_db, this);

            List<ClassAllocation> classAllocations = aspUser.User.List_ClassAllocation.Where(e => e.Deleted == false).OrderByDescending(e => e.DateCreated).ToList();
            MyAttendanceViewModel model = new MyAttendanceViewModel();
            List<MyAttendanceViewItem> attendanceViewItems = new List<MyAttendanceViewItem>();
            foreach (ClassAllocation item in classAllocations)
            {
                List<Attendance> classAttendances = item.Class.List_Attendances.Where(e => e.Deleted == false).ToList();
                List<AttendanceItem> studentAttendances = item.Student.List_AttendanceItems.Where(e => classAttendances.Contains(e.Attendance)).ToList();
                MyAttendanceViewItem newMyAttendanceItem = new MyAttendanceViewItem()
                {
                    ClassName = item.Class.Name,
                    Attendance = studentAttendances.Count().ToString() + "/" + classAttendances.Count().ToString(),
                    DateJoined = item.DateCreated
                };

                attendanceViewItems.Add(newMyAttendanceItem);
            }

            model.StudentName = aspUser.User.Name;
            model.Classes = attendanceViewItems;
            return View(model);
        }
    }
}
