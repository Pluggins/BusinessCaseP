using BCP_Facial.Data;
using BCP_Facial.Models;
using BCP_Facial.Models.ViewModels;
using BCP_Facial.Services;
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
        public async Task<ClassInfoOutput> Create([FromBody] ClassInfoInput input)
        {
            ClassInfoOutput output = new ClassInfoOutput();
            AspUserService aspUser = new AspUserService(_db, this);

            if (aspUser.IsAdmin)
            {
                BCPUser lecturer = _db._BCPUsers.Where(e => e.Id.Equals(input.LecturerId) && e.Deleted == false && e.Status >= 3).FirstOrDefault();

                if (lecturer == null)
                {
                    Response.StatusCode = 400;
                    output.Result = "USER_NOT_FOUND";
                } else
                {
                    string pgId = Guid.NewGuid().ToString();
                    string personGroupId = Guid.NewGuid().ToString();
                    Uri uri = new Uri("https://bcp-facial.cognitiveservices.azure.com/" + "face/v1.0/persongroups/"+ pgId);
                    HttpClientHandler handler = new HttpClientHandler();
                    StringContent queryString = null;
                    HttpClient client = new HttpClient(handler);
                    //string respond = null;

                    HttpResponseMessage response = await client.PostAsync(uri, queryString);
                    //respond = await response.Content.ReadAsStringAsync();

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        PersonGroupInput pgInput = new PersonGroupInput()
                        {
                            Name = "Class-" + input.ClassName,
                            UserData = "For class of " + input.ClassName
                        };

                        queryString = new StringContent(JsonConvert.SerializeObject(pgInput), Encoding.UTF8, "application/json");

                        Class newClass = new Class()
                        {
                            Name = input.ClassName,
                            Lecturer = lecturer,
                            CreatedBy = aspUser.User.Id,
                            PersonGroupId = pgId
                        };

                        _db.Classes.Add(newClass);
                        Response.StatusCode = 200;
                        output.Result = "OK";
                    } else
                    {
                        Response.StatusCode = 500;
                        output.Result = "INTERNAL_ERROR";
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
