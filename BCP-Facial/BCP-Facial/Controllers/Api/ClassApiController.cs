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

        [HttpPost]
        [Route("Api/Class/Change")]
        public ClassInfoOutput Change([FromBody] ClassInfoInput input)
        {
            ClassInfoOutput output = new ClassInfoOutput();

            if (input == null)
            {
                Response.StatusCode = 400;
                output.Result = "INPUT_IS_NULL";
            } else
            {
                AspUserService aspUser = new AspUserService(_db, this);
                if (aspUser.IsAdmin)
                {
                    Class thisClass = _db.Classes.Where(e => e.Id.Equals(input.ClassId) && e.Deleted == false).FirstOrDefault();
                    if (thisClass == null)
                    {
                        Response.StatusCode = 400;
                        output.Result = "CLASS_NOT_EXIST";
                    } else
                    {
                        BCPUser lecturer = _db._BCPUsers.Where(e => e.Id.Equals(input.LecturerId) && e.Deleted == false).Where(e => e.Status == 2 || e.Status == 4).FirstOrDefault();
                        if (lecturer == null && !string.IsNullOrEmpty(input.LecturerId))
                        {
                            Response.StatusCode = 400;
                            output.Result = "LECTURER_NOT_EXIST";
                        } else
                        {
                            thisClass.Capacity = input.Capacity;
                            thisClass.Name = input.ClassName;
                            if (string.IsNullOrEmpty(input.LecturerId))
                            {
                                thisClass.Lecturer = null;
                            } else
                            {
                                thisClass.Lecturer = lecturer;
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
            }

            return output;
        }

        [HttpPost]
        [Route("Api/Class/RemoveStudent")]
        public ClassInfoOutput RemoveStudent([FromBody] ClassInfoInput input)
        {
            ClassInfoOutput output = new ClassInfoOutput();

            if (input == null)
            {
                Response.StatusCode = 400;
                output.Result = "INPUT_IS_NULL";
            } else
            {
                AspUserService aspUser = new AspUserService(_db, this);
                if (aspUser.IsAdmin)
                {
                    Class thisClass = _db.Classes.Where(e => e.Id.Equals(input.ClassId) && e.Deleted == false).FirstOrDefault();
                    if (thisClass == null)
                    {
                        Response.StatusCode = 400;
                        output.Result = "CLASS_NOT_EXIST";
                    } else
                    {
                        List<ClassAllocation> classAllocationList = thisClass.List_ClassAllocation.Where(e => e.Student.Id.Equals(input.StudentId) && e.Deleted == false).ToList();
                        if (classAllocationList.Count() > 0)
                        {
                            foreach(ClassAllocation item in classAllocationList)
                            {
                                item.Deleted = true;
                            }

                            _db.SaveChanges();
                            output.Result = "OK";
                        } else
                        {
                            Response.StatusCode = 400;
                            output.Result = "STUDENT_NOT_IN_CLASS";
                        }
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
        [Route("Api/Class/RetrievePendingPhoto")]
        public ClassPendingPhotoOutput RetrievePendingPhoto([FromBody] ClassPendingPhotoInput input)
        {
            ClassPendingPhotoOutput output = new ClassPendingPhotoOutput();

            if (input == null)
            {
                Response.StatusCode = 400;
                output.Result = "INPUT_IS_NULL";
            } else
            {
                AspUserService aspUser = new AspUserService(_db, this);
                if (aspUser.IsLecturer)
                {
                    Class thisClass = aspUser.User.List_Classes.Where(e => e.Id.Equals(input.ClassId) && e.Deleted == false).FirstOrDefault();
                    if (thisClass == null)
                    {
                        Response.StatusCode = 400;
                        output.Result = "CLASS_NOT_EXIST";
                    } else
                    {
                        string siteUrl = _db.SiteConfigs.Where(e => e.Key.Equals("SITEURL")).FirstOrDefault().Value;
                        List<GroupImage> images = thisClass.List_GroupImages.Where(e => e.Status == 1 && e.Deleted == false).ToList();
                        List<ClassPendingPhotoItem> photos = new List<ClassPendingPhotoItem>();
                        foreach (GroupImage item in images)
                        {
                            ClassPendingPhotoItem photoItem = new ClassPendingPhotoItem()
                            {
                                Url = siteUrl + "/"+item.Url
                            };
                            photos.Add(photoItem);
                        }

                        output.Photos = photos;
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

        [AllowAnonymous]
        [HttpPost]
        [Route("Api/Class/AddPhoto")]
        public ClassInfoOutput AddPhoto([FromBody] ClassInfoInput input)
        {
            ClassInfoOutput output = new ClassInfoOutput();

            if (input == null)
            {
                Response.StatusCode = 400;
                output.Result = "INPUT_IS_NULL";
            } else
            {
                AspUserService aspUser = new AspUserService(_db, this);
                Recognizer recognizer = _db.Recognizers.Where(e => e.Id.Equals(input.RecognizerId) && e.Deleted == false).FirstOrDefault();
                if (recognizer == null)
                {
                    Response.StatusCode = 400;
                    output.Result = "RECOGNIZER_NOT_EXIST";
                } else
                {
                    if (recognizer.Key.Equals(input.RecognizerKey))
                    {
                        Class thisClass = _db.Classes.Where(e => e.Id.Equals(input.ClassId) && e.Deleted == false).FirstOrDefault();
                        if (thisClass == null)
                        {
                            Response.StatusCode = 400;
                            output.Result = "CLASS_NOT_EXIST";
                        } else
                        {
                            GroupImage gi = new GroupImage
                            {
                                Url = input.ImageUrl,
                                Class = thisClass,
                                CreatedBy = recognizer.Id,
                                Status = 1
                            };

                            _db.GroupImages.Add(gi);
                            _db.SaveChanges();
                            output.Result = "OK";
                        }
                    } else
                    {
                        Response.StatusCode = 400;
                        output.Result = "CREDENTIAL_ERROR";
                    }
                }
            }

            return output;
        }
    }
}
