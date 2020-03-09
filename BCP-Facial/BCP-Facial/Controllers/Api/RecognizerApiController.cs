using BCP_Facial.Data;
using BCP_Facial.Models;
using BCP_Facial.Models.ViewModels;
using BCP_Facial.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace BCP_Facial.Controllers.Api
{
    [Authorize]
    public class RecognizerApiController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public RecognizerApiController(
            ApplicationDbContext db,
            IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Api/Recognizer/UploadImage")]
        public async Task<ImageUploadOutput> UploadImage()
        {
            ImageUploadOutput output = new ImageUploadOutput();

            var file = Request.Form.Files;
            foreach (var item in file)
            {
                if (item != null && item.Length > 0)
                {
                    string id = Guid.NewGuid().ToString();
                    string[] ext = item.FileName.Split('.');
                    string part1 = "wwwroot";
                    string part2 = "recognizer\\" + id + "." + ext[ext.Length - 1];
                    string fileName = Path.Combine(part1, part2);
                    string url = _hostingEnvironment.ContentRootPath;
                    string path = Path.Combine(url, fileName);

                    using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
                    {
                        await item.CopyToAsync(fs);
                        output.Url = "recognizer/" + id + "." + ext[ext.Length - 1];
                    }
                }
            }
            output.Result = "OK";

            return output;
        }

        [HttpPost]
        [Route("Api/Recognizer/GetStatus")]
        public async Task<RecognizerInfoOutput> GetStatus([FromBody] RecognizerInfoInput input)
        {
            RecognizerInfoOutput output = new RecognizerInfoOutput();

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
                    RecognizerService recogService = new RecognizerService(input.RecognizerId, _db);

                    if (!recogService.IsExist)
                    {
                        Response.StatusCode = 400;
                        output.Result = "RECOGNIZER_NOT_EXIST";
                    }
                    else
                    {
                        string trainingStatus = "NONE";
                        if (!string.IsNullOrEmpty(input.Type))
                        {
                            if (input.Type.Equals("REGISTER_NEW_FACE"))
                            {
                                string personGroupId = _db.SiteConfigs.Where(e => e.Key.Equals("PERSONGROUP")).FirstOrDefault().Value;
                                Uri uri = new Uri("https://bcp-facial.cognitiveservices.azure.com/face/v1.0/persongroups/" + personGroupId + "/training");
                                HttpClientHandler handler = new HttpClientHandler();
                                HttpClient client = new HttpClient(handler);
                                //string respond = null;
                                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "8d4c42ecbb784d909275492115ea56f0");
                                HttpResponseMessage response = await client.GetAsync(uri);
                                string siteUrl = _db.SiteConfigs.Where(e => e.Key.Equals("SITEURL")).First().Value;
                                string respond = await response.Content.ReadAsStringAsync();
                                dynamic jsonObj = JsonConvert.DeserializeObject(respond);
                                if (jsonObj.status != null)
                                {
                                    trainingStatus = jsonObj.status;
                                }
                                else
                                {
                                    trainingStatus = "NONE";
                                }
                            }
                        }
                        output.RecognizerId = recogService.Recognizer.Id;
                        output.RecognizerStatus = recogService.GetStatus();
                        output.LastActivity = recogService.GetDurationSince();

                        if (trainingStatus == "running")
                        {
                            output.Result = "TRAINING_IN_PROGRESS";
                        } else
                        {
                            output.Result = "OK";
                        }
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

        [AllowAnonymous]
        [HttpPost]
        [Route("Api/Recognizer/FetchInstruction")]
        public RecognizerInfoOutput FetchInstruction([FromBody] RecognizerInfoInput input)
        {
            RecognizerInfoOutput output = new RecognizerInfoOutput();

            if (input == null)
            {
                Response.StatusCode = 400;
                output.Result = "INPUT_IS_NULL";
            } else
            {
                RecognizerService recogService = new RecognizerService(input.RecognizerId, input.RecognizerKey, _db);
                if (recogService.IsAuthentic)
                {
                    RecognizerTask task = recogService.GetLastUnreadTask();
                    if (task == null)
                    {
                        output.Result = "NO_NEW_TASK";
                    } else
                    {
                        task.Status = 2;
                        _db.SaveChanges();

                        output.TaskId = task.Id;
                        output.Command = task.Command;
                        output.PrimaryValue = task.PrimaryValue;
                        output.SecondaryValue = task.SecondaryValue;
                        output.Result = "OK";
                    }
                } else
                {
                    Response.StatusCode = 400;
                    output.Result = "CREDENTIAL_ERROR";
                }
            }

            return output;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Api/Recognizer/MarkTaskComplete")]
        public RecognizerInfoOutput MarkTaskComplete([FromBody] RecognizerInfoInput input)
        {
            RecognizerInfoOutput output = new RecognizerInfoOutput();

            if (input == null)
            {
                Response.StatusCode = 400;
                output.Result = "INPUT_IS_NULL";
            } else
            {
                RecognizerService recogService = new RecognizerService(input.RecognizerId, input.RecognizerKey, _db);
                if (recogService.IsAuthentic)
                {
                    RecognizerTask task = recogService.Recognizer.List_RecognizerTask.Where(e => e.Id.Equals(input.RecognizerTaskId) && e.Deleted == false).FirstOrDefault();
                    if (task == null)
                    {
                        Response.StatusCode = 400;
                        output.Result = "TASK_NOT_EXIST";
                    } else
                    {
                        task.Status = 3;
                        _db.SaveChanges();

                        output.Result = "OK";
                    }
                } else
                {
                    Response.StatusCode = 400;
                    output.Result = "CREDENTIAL_ERROR";
                }
            }
            return output;
        }
    }
}
