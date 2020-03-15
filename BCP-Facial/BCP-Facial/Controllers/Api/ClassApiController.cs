using BCP_Facial.Data;
using BCP_Facial.Models;
using BCP_Facial.Models.ViewModels;
using BCP_Facial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BCP_Facial.Controllers.Api
{
    [Authorize]
    public class ClassApiController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ClassApiController(
            ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        [Route("Api/Class/Create")]
        public ClassInfoOutput Create([FromBody] ClassInfoInput input)
        {
            ClassInfoOutput output = new ClassInfoOutput();
            AspUserService aspUser = new AspUserService(_db, this);
            
            if (aspUser.IsAdmin)
            {
                if (input == null)
                {
                    Response.StatusCode = 400;
                    output.Result = "INPUT_IS_NULL";
                } else
                {
                    if (string.IsNullOrEmpty(input.ClassName) || string.IsNullOrEmpty(input.ClassCode))
                    {
                        Response.StatusCode = 400;
                        output.Result = "INPUT_IS_NULL";
                    } else
                    {
                        Class thisClass = _db.Classes.Where(e => e.ClassCode.ToUpper().Equals(input.ClassCode.ToUpper()) && e.Deleted == false).FirstOrDefault();

                        if (thisClass == null)
                        {
                            Class newClass = new Class()
                            {
                                Name = input.ClassName,
                                ClassCode = input.ClassCode.ToUpper(),
                                CreatedBy = aspUser.User.Id
                            };

                            output.Result = "OK";

                            _db.Classes.Add(newClass);
                            _db.SaveChanges();
                        } else
                        {
                            Response.StatusCode = 400;
                            output.Result = "CLASS_EXIST";
                        }
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
        [Route("Api/Class/CheckClassById")]
        public ClassInfoOutput CheckClassById([FromBody] ClassInfoInput input)
        {
            ClassInfoOutput output = new ClassInfoOutput();
            AspUserService aspUser = new AspUserService(_db, this);

            if (input == null)
            {
                Response.StatusCode = 400;
                output.Result = "INPUT_IS_NULL";
            } else
            {
                if (aspUser.IsAdmin)
                {
                    Class selectedClass = _db.Classes.Where(e => e.Id.Equals(input.ClassId)).FirstOrDefault();
                    if (selectedClass == null)
                    {
                        Response.StatusCode = 400;
                        output.Result = "CLASS_NOT_EXIST";
                    } else
                    {
                        output.ClassName = selectedClass.Name;
                        output.Result = "OK";
                    }
                } else
                {
                    Response.StatusCode = 400;
                    output.Result = "NO_PRIVILEGE";
                }
            }

            return output;
        }

        [HttpPost]
        [Route("Api/Class/Remove")]
        public ClassInfoOutput Remove([FromBody] ClassInfoInput input)
        {
            ClassInfoOutput output = new ClassInfoOutput();
            AspUserService aspUser = new AspUserService(_db, this);

            if (input == null)
            {
                Response.StatusCode = 400;
                output.Result = "INPUT_IS_NULL";
            }
            else
            {
                if (aspUser.IsAdmin)
                {
                    Class selectedClass = _db.Classes.Where(e => e.Id.Equals(input.ClassId)).FirstOrDefault();
                    selectedClass.Deleted = true;
                    output.Result = "OK";
                    _db.SaveChanges();
                } else
                {
                    Response.StatusCode = 400;
                    output.Result = "NO_PRIVILEGE";
                }
            }

            return output;
        }
    }
}
