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

namespace BCP_Facial.Controllers.Api
{
    [Authorize]
    public class LecturerApiController : Controller
    {
        private readonly ApplicationDbContext _db;

        public LecturerApiController(
            ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        [Route("Api/Lecturer/AddByEmail")]
        public UserInfoOutput AddByEmail([FromBody] UserInfoInput input)
        {
            AspUserService aspUser = new AspUserService(_db, this);
            UserInfoOutput output = new UserInfoOutput();

            if (aspUser.IsAdmin)
            {
                BCPUser user = _db._BCPUsers.Where(e => e.Email.ToUpper().Equals(input.Email.ToUpper()) && e.Deleted == false).FirstOrDefault();
                if (user == null)
                {
                    Response.StatusCode = 400;
                    output.Result = "USER_NOT_EXIST";
                } else
                {
                    if (user.Status == 2 || user.Status == 4)
                    {
                        Response.StatusCode = 400;
                        output.Result = "USER_ALREADY_ASSIGNED_LECTURER";
                    } else
                    {
                        if (user.Status == 3)
                        {
                            user.Status = 4;
                        } else
                        {
                            user.Status = 2;
                        }
                        _db.SaveChanges();

                        output.Result = "OK";
                    }
                }
            } else
            {
                Response.StatusCode = 400;
                output.Result = "NO_PRIVILEGE";
            }
            

            return output;
        }

        [HttpPost]
        [Route("Api/Lecturer/Remove")]
        public UserInfoOutput Remove([FromBody] UserInfoInput input)
        {
            UserInfoOutput output = new UserInfoOutput();
            AspUserService aspUser = new AspUserService(_db, this);

            if (aspUser.IsAdmin)
            {
                if (input == null)
                {
                    Response.StatusCode = 400;
                    output.Result = "INPUT_IS_NULL";
                } else
                {
                    BCPUser user = _db._BCPUsers.Where(e => e.Id.Equals(input.UserId)).FirstOrDefault();
                    if (user == null)
                    {
                        Response.StatusCode = 400;
                        output.Result = "USER_NOT_EXIST";
                    } else
                    {
                        if (user.Status == 4)
                        {
                            user.Status = 3;
                        } else
                        {
                            user.Status = 1;
                        }

                        _db.SaveChanges();
                        output.Result = "OK";
                    }
                }
            } else
            {
                Response.StatusCode = 400;
                output.Result = "NO_PRIVILEGE";
            }
            return output;
        }
    }
}
