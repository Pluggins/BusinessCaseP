using BCP_Facial.Data;
using BCP_Facial.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BCP_Facial.Services
{
    public class AspUserService
    {
        public bool IsValid { get; set; }
        public bool IsLecturer { get; set; }
        public bool IsAdmin { get; set; }
        public BCPUser User { get; set; }

        public AspUserService(ApplicationDbContext db, Controller controller)
        {
            User = db._BCPUsers.Where(e => e.AspUser.Id.Equals(controller.User.FindFirstValue(ClaimTypes.NameIdentifier)) && e.Deleted == false).FirstOrDefault();
            IsValid = false;
            IsAdmin = false;
            IsLecturer = false;

            if (User != null)
            {
                if (User.Status > 0)
                {
                    IsValid = true;
                }

                if (User.Status == 2)
                {
                    IsLecturer = true;
                }

                if (User.Status == 3)
                {
                    IsAdmin = true;
                }

                if (User.Status == 4)
                {
                    IsLecturer = true;
                    IsAdmin = true;
                }
            }
        }
    }
}
