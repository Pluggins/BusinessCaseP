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
                    if (string.IsNullOrEmpty(input.ClassName))
                    {
                        Response.StatusCode = 400;
                        output.Result = "INPUT_IS_NULL";
                    } else
                    {
                        Class newClass = new Class()
                        {
                            Name = input.ClassName,
                            CreatedBy = aspUser.User.Id
                        };

                        output.Result = "OK";

                        _db.Classes.Add(newClass);
                        _db.SaveChanges();
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
