using BCP_Facial.Data;
using BCP_Facial.Models;
using BCP_Facial.Models.ViewModels;
using BCP_Facial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCP_Facial.Controllers.Api
{
    [Authorize]
    public class RecognizerTaskApiController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public RecognizerTaskApiController(
            ApplicationDbContext db,
            IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpPost]
        [Route("Api/RecognizerTask/StudentImageCapture")]
        public RecognizerTaskOutput StudentImageCapture([FromBody] RecognizerTaskInput input)
        {
            RecognizerTaskOutput output = new RecognizerTaskOutput();

            if (input == null)
            {
                Response.StatusCode = 400;
                output.Result = "INPUT_IS_NULL";
            }
            else
            {
                AspUserService aspUser = new AspUserService(_db, this);
                if (aspUser.IsAdmin)
                {
                    Recognizer recognizer = _db.Recognizers.Where(e => e.Id.Equals(input.RecognizerId) && e.Deleted == false).FirstOrDefault();
                    BCPUser student = _db._BCPUsers.Where(e => e.Id.Equals(input.StudentId) && e.Deleted == false).FirstOrDefault();

                    if (recognizer == null)
                    {
                        Response.StatusCode = 400;
                        output.Result = "RECOGNIZER_NOT_FOUND";
                    }
                    else if (student == null)
                    {
                        Response.StatusCode = 400;
                        output.Result = "STUDENT_NOT_FOUND";
                    }
                    else
                    {
                        List<UserImage> studentImages = student.List_UserImage.Where(e => e.Deleted == false && e.Status != 0).ToList();
                        foreach (UserImage item in studentImages)
                        {
                            item.Status = 0;
                        }

                        RecognizerTask task = new RecognizerTask()
                        {
                            Command = "REGISTER_NEW_FACE",
                            Status = 1,
                            Recognizer = recognizer,
                            PrimaryValue = student.Id,
                            SecondaryValue = (int.Parse(_db.SiteConfigs.Where(e => e.Key.Equals("NUM_PHOTO_PER_STUDENT")).FirstOrDefault().Value) + 5).ToString()
                        };

                        _db.RecognizerTasks.Add(task);
                        _db.SaveChanges();

                        output.RecognizerTaskId = task.Id;
                        output.Result = "OK";
                    }

                }
                else
                {
                    Response.StatusCode = 400;
                    output.Result = "NO_PRIVILEGE";
                }
            }

            return output;
        }

        [HttpPost]
        [Route("Api/RecognizerTask/GroupImageCapture")]
        public RecognizerTaskOutput GroupImageCapture([FromBody] RecognizerTaskInput input)
        {
            RecognizerTaskOutput output = new RecognizerTaskOutput();

            if (input == null)
            {
                Response.StatusCode = 400;
                output.Result = "INPUT_IS_NULL";
            }
            else
            {
                AspUserService aspUser = new AspUserService(_db, this);
                if (aspUser.IsAdmin)
                {
                    Recognizer recognizer = _db.Recognizers.Where(e => e.Id.Equals(input.RecognizerId) && e.Deleted == false).FirstOrDefault();
                    Class thisClass = aspUser.User.List_Classes.Where(e => e.Id.Equals(input.ClassId) && e.Deleted == false).FirstOrDefault();

                    if (recognizer == null)
                    {
                        Response.StatusCode = 400;
                        output.Result = "RECOGNIZER_NOT_FOUND";
                    }
                    else if (thisClass == null)
                    {
                        Response.StatusCode = 400;
                        output.Result = "CLASS_NOT_FOUND";
                    }
                    else
                    {
                        List<GroupImage> groupImages = thisClass.List_GroupImages.Where(e => e.Deleted == false && e.Status != 0).ToList();
                        foreach (GroupImage item in groupImages)
                        {
                            item.Status = 0;
                        }

                        RecognizerTask task = new RecognizerTask()
                        {
                            Command = "CAPTURE_CLASS_IMAGE",
                            Status = 1,
                            Recognizer = recognizer,
                            PrimaryValue = thisClass.Id,
                            SecondaryValue = (int.Parse(_db.SiteConfigs.Where(e => e.Key.Equals("NUM_PHOTO_PER_CLASS")).FirstOrDefault().Value) + 5).ToString()
                        };

                        _db.RecognizerTasks.Add(task);
                        _db.SaveChanges();

                        output.RecognizerTaskId = task.Id;
                        output.Result = "OK";
                    }

                }
                else
                {
                    Response.StatusCode = 400;
                    output.Result = "NO_PRIVILEGE";
                }
            }

            return output;
        }

        [HttpPost]
        [Route("Api/RecognizerTask/CheckStatusById")]
        public RecognizerTaskOutput CheckStatusById([FromBody] RecognizerTaskInput input)
        {
            RecognizerTaskOutput output = new RecognizerTaskOutput();

            if (input == null)
            {
                Response.StatusCode = 400;
                output.Result = "INPUT_IS_NULL";
            } else
            {
                AspUserService aspUser = new AspUserService(_db, this);
                if (aspUser.IsAdmin)
                {
                    RecognizerTask task = _db.RecognizerTasks.Where(e => e.Id.Equals(input.RecognizerTaskId) && e.Deleted == false).FirstOrDefault();
                    if (task == null)
                    {
                        Response.StatusCode = 400;
                        output.Result = "TASK_NOT_EXIST";
                    } else
                    {
                        if (task.Status == 0)
                        {
                            output.Status = "CANCELLED";
                        } else if (task.Status == 1)
                        {
                            output.Status = "UNREAD";
                        } else if (task.Status == 2)
                        {
                            output.Status = "READ";
                        } else if (task.Status == 3)
                        {
                            output.Status = "DONE";
                        }
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
    }
}
