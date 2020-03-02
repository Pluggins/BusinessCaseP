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
    public class StudentApiController : Controller
    {
        private readonly ApplicationDbContext _db;

        public StudentApiController(
            ApplicationDbContext db)
        {
            _db = db;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Api/Student/AddPhoto")]
        public StudentInfoOutput AddPhoto([FromBody] StudentInfoInput input)
        {
            StudentInfoOutput output = new StudentInfoOutput();
            if (input == null)
            {
                Response.StatusCode = 400;
                output.Result = "INPUT_IS_NULL";
            } else
            {
                Recognizer recognizer = _db.Recognizers.Where(e => e.Id.Equals(input.RecognizerId)).FirstOrDefault();
                if (recognizer == null)
                {
                    Response.StatusCode = 400;
                    output.Result = "CREDENTIAL_ERROR";
                } else
                {
                    if (recognizer.Key.Equals(input.RecognizerKey))
                    {
                        UserImage userImage = new UserImage()
                        {
                            User = _db._BCPUsers.Where(e => e.Id.Equals(input.StudentId)).FirstOrDefault(),
                            Url = input.ImageUrl,
                            Status = 1,
                            CreatedBy = recognizer.Id
                        };

                        _db.UserImages.Add(userImage);
                        _db.SaveChanges();
                        output.Result = "OK";
                    } else
                    {
                        Response.StatusCode = 400;
                        output.Result = "CREDENTIAL_ERROR";
                    }
                }
            }

            return output;
        }

        [HttpPost]
        [Route("Api/Student/ProcessPhoto")]
        public async Task<StudentInfoOutput> ProcessPhoto([FromBody] StudentInfoInput input)
        {
            StudentInfoOutput output = new StudentInfoOutput();
            AspUserService aspUser = new AspUserService(_db, this);
            bool failure = false;

            if (input == null)
            {
                Response.StatusCode = 400;
                output.Result = "INPUT_IS_NULL";
            } else
            {
                string personGroupId = _db.SiteConfigs.Where(e => e.Key.Equals("PERSONGROUP")).FirstOrDefault().Value;
                Uri uri = new Uri("https://bcp-facial.cognitiveservices.azure.com/face/v1.0/persongroups/" + personGroupId + "/training");
                HttpClientHandler handler = new HttpClientHandler();
                StringContent queryString = null;
                HttpClient client = new HttpClient(handler);
                //string respond = null;
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "8d4c42ecbb784d909275492115ea56f0");
                HttpResponseMessage response = await client.GetAsync(uri);
                string respond = await response.Content.ReadAsStringAsync();
                dynamic jsonObj = JsonConvert.DeserializeObject(respond);
                string trainingStatus = "NONE";
                if (jsonObj.status != null)
                {
                    trainingStatus = jsonObj.status;
                } else
                {
                    trainingStatus = "NONE";
                }

                if (trainingStatus.Equals("running"))
                {
                    Response.StatusCode = 400;
                    output.Result = "TRAINING_IN_PROGRESS";
                } else
                {
                    if (aspUser.IsAdmin)
                    {
                        BCPUser user = _db._BCPUsers.Where(e => e.Id.Equals(input.StudentId)).FirstOrDefault();

                        if (user == null)
                        {
                            Response.StatusCode = 400;
                            output.Result = "USER_NOT_FOUND";
                        }
                        else
                        {
                            List<UserImage> imageStore = user.List_UserImage.Where(e => e.Status == 1).OrderBy(e => e.DateCreated).ToList();
                            List<string> imageToBeUsed = new List<string>();
                            int min = int.Parse(_db.SiteConfigs.Where(e => e.Key.Equals("NUM_PHOTO_PER_STUDENT")).FirstOrDefault().Value);
                            string siteUrl = _db.SiteConfigs.Where(e => e.Key.Equals("SITEURL")).FirstOrDefault().Value;
                            int imageCount = imageStore.Count;
                            string faceIdToCompare = null;

                            if (imageCount >= min)
                            {
                                string faceListId = Guid.NewGuid().ToString().ToLower();
                                uri = new Uri("https://bcp-facial.cognitiveservices.azure.com/face/v1.0/facelists/" + faceListId);
                                handler = new HttpClientHandler();
                                queryString = null;
                                client = new HttpClient(handler);
                                //string respond = null;
                                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "8d4c42ecbb784d909275492115ea56f0");
                                PersonGroupInput pgInput = new PersonGroupInput()
                                {
                                    Name = faceListId,
                                    RecognitionModel = "recognition_02"
                                };

                                queryString = new StringContent(JsonConvert.SerializeObject(pgInput), Encoding.UTF8, "application/json");
                                queryString.Headers.Remove("Content-Type");
                                queryString.Headers.Add("Content-Type", "application/json");
                                response = await client.PutAsync(uri, queryString);
                                //respond = await response.Content.ReadAsStringAsync();

                                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    foreach (UserImage item in imageStore)
                                    {
                                        if (imageCount > 1)
                                        {
                                            uri = new Uri("https://bcp-facial.cognitiveservices.azure.com/face/v1.0/facelists/" + faceListId + "/persistedFaces");
                                            handler = new HttpClientHandler();
                                            queryString = null;
                                            client = new HttpClient(handler);
                                            //string respond = null;
                                            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "8d4c42ecbb784d909275492115ea56f0");
                                            pgInput = new PersonGroupInput()
                                            {
                                                Url = item.Url
                                            };

                                            queryString = new StringContent(JsonConvert.SerializeObject(pgInput), Encoding.UTF8, "application/json");
                                            queryString.Headers.Remove("Content-Type");
                                            queryString.Headers.Add("Content-Type", "application/json");
                                            response = await client.PostAsync(uri, queryString);
                                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                            {
                                                respond = await response.Content.ReadAsStringAsync();

                                                jsonObj = JsonConvert.DeserializeObject(respond);
                                                item.FaceId = jsonObj.persistedFaceId;
                                            }

                                            _db.SaveChanges();
                                            imageCount--;
                                        }
                                        else
                                        {
                                            uri = new Uri("https://bcp-facial.cognitiveservices.azure.com/face/v1.0/detect?recognitionModel=recognition_02");
                                            handler = new HttpClientHandler();
                                            queryString = null;
                                            client = new HttpClient(handler);
                                            //string respond = null;
                                            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "8d4c42ecbb784d909275492115ea56f0");
                                            pgInput = new PersonGroupInput()
                                            {
                                                Url = item.Url
                                            };

                                            queryString = new StringContent(JsonConvert.SerializeObject(pgInput), Encoding.UTF8, "application/json");
                                            queryString.Headers.Remove("Content-Type");
                                            queryString.Headers.Add("Content-Type", "application/json");
                                            response = await client.PostAsync(uri, queryString);

                                            if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                            {
                                                respond = await response.Content.ReadAsStringAsync();

                                                jsonObj = JsonConvert.DeserializeObject(respond);
                                                item.Status = 0;
                                                faceIdToCompare = jsonObj[0].faceId;
                                            }

                                            _db.SaveChanges();
                                        }
                                    }

                                    uri = new Uri("https://bcp-facial.cognitiveservices.azure.com/face/v1.0/findsimilars");
                                    handler = new HttpClientHandler();
                                    queryString = null;
                                    client = new HttpClient(handler);
                                    //string respond = null;
                                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "8d4c42ecbb784d909275492115ea56f0");
                                    pgInput = new PersonGroupInput()
                                    {
                                        FaceId = faceIdToCompare,
                                        FaceListId = faceListId,
                                        maxNumOfCandidatesReturned = 10
                                    };

                                    queryString = new StringContent(JsonConvert.SerializeObject(pgInput), Encoding.UTF8, "application/json");
                                    queryString.Headers.Remove("Content-Type");
                                    queryString.Headers.Add("Content-Type", "application/json");
                                    response = await client.PostAsync(uri, queryString);
                                    respond = await response.Content.ReadAsStringAsync();

                                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                    {
                                        respond = await response.Content.ReadAsStringAsync();

                                        jsonObj = JsonConvert.DeserializeObject(respond);
                                        foreach (var item in jsonObj)
                                        {
                                            string id = item.persistedFaceId;
                                            UserImage ui = _db.UserImages.Where(e => e.FaceId.Equals(id)).FirstOrDefault();
                                            ui.Confidence = item.confidence;
                                            imageToBeUsed.Add(id);
                                            _db.SaveChanges();
                                        }
                                    }
                                    else
                                    {
                                        failure = true;
                                    }

                                    uri = new Uri("https://bcp-facial.cognitiveservices.azure.com/face/v1.0/facelists/" + faceListId);
                                    handler = new HttpClientHandler();
                                    queryString = null;
                                    client = new HttpClient(handler);
                                    //string respond = null;
                                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "8d4c42ecbb784d909275492115ea56f0");

                                    queryString = new StringContent("", Encoding.UTF8, "application/json");
                                    queryString.Headers.Remove("Content-Type");
                                    queryString.Headers.Add("Content-Type", "application/json");
                                    response = await client.DeleteAsync(uri);

                                    if (imageToBeUsed.Count < min)
                                    {
                                        Response.StatusCode = 500;
                                        output.Result = "NUM_PHOTO_NOT_SUFFICIENT";
                                    } else
                                    {
                                        if (user.PersonId != null)
                                        {
                                            uri = new Uri("https://bcp-facial.cognitiveservices.azure.com/face/v1.0/persongroups/" + personGroupId + "/persons/" + user.PersonId);
                                            handler = new HttpClientHandler();
                                            queryString = null;
                                            client = new HttpClient(handler);
                                            //string respond = null;
                                            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "8d4c42ecbb784d909275492115ea56f0");

                                            queryString = new StringContent("", Encoding.UTF8, "application/json");
                                            queryString.Headers.Remove("Content-Type");
                                            queryString.Headers.Add("Content-Type", "application/json");
                                            response = await client.DeleteAsync(uri);
                                        }

                                        if (response.StatusCode != System.Net.HttpStatusCode.OK)
                                        {
                                            failure = true;
                                        }

                                        uri = new Uri("https://bcp-facial.cognitiveservices.azure.com/face/v1.0/persongroups/" + personGroupId + "/persons");
                                        handler = new HttpClientHandler();
                                        queryString = null;
                                        client = new HttpClient(handler);
                                        //string respond = null;
                                        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "8d4c42ecbb784d909275492115ea56f0");
                                        pgInput = new PersonGroupInput()
                                        {
                                            Name = user.Id
                                        };
                                        queryString = new StringContent(JsonConvert.SerializeObject(pgInput), Encoding.UTF8, "application/json");
                                        queryString.Headers.Remove("Content-Type");
                                        queryString.Headers.Add("Content-Type", "application/json");
                                        response = await client.PostAsync(uri, queryString);

                                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                        {
                                            respond = await response.Content.ReadAsStringAsync();
                                            jsonObj = JsonConvert.DeserializeObject(respond);
                                            user.PersonId = jsonObj.personId;
                                            _db.SaveChanges();
                                        }
                                        else
                                        {
                                            failure = true;
                                        }

                                        foreach (UserImage item in imageStore)
                                        {
                                            item.Status = 0;
                                            _db.SaveChanges();
                                        }

                                        foreach (string s in imageToBeUsed)
                                        {
                                            if (min > 0)
                                            {
                                                UserImage item = imageStore.Where(e => e.FaceId.Equals(s)).FirstOrDefault();
                                                uri = new Uri("https://bcp-facial.cognitiveservices.azure.com/face/v1.0/persongroups/" + personGroupId + "/persons/" + user.PersonId + "/persistedFaces");
                                                handler = new HttpClientHandler();
                                                queryString = null;
                                                client = new HttpClient(handler);
                                                //string respond = null;
                                                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "8d4c42ecbb784d909275492115ea56f0");
                                                pgInput = new PersonGroupInput()
                                                {
                                                    Url = item.Url
                                                };
                                                queryString = new StringContent(JsonConvert.SerializeObject(pgInput), Encoding.UTF8, "application/json");
                                                queryString.Headers.Remove("Content-Type");
                                                queryString.Headers.Add("Content-Type", "application/json");
                                                response = await client.PostAsync(uri, queryString);

                                                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                                {
                                                    respond = await response.Content.ReadAsStringAsync();
                                                    jsonObj = JsonConvert.DeserializeObject(respond);
                                                    item.FaceId = jsonObj.persistedFaceId;
                                                    item.Status = 2;
                                                    _db.SaveChanges();
                                                }
                                                else
                                                {
                                                    failure = true;
                                                }
                                            }
                                            min--;
                                        }

                                        uri = new Uri("https://bcp-facial.cognitiveservices.azure.com/face/v1.0/persongroups/" + personGroupId + "/train");
                                        handler = new HttpClientHandler();
                                        queryString = null;
                                        client = new HttpClient(handler);
                                        //string respond = null;
                                        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "8d4c42ecbb784d909275492115ea56f0");
                                        queryString = new StringContent("", Encoding.UTF8, "application/json");
                                        queryString.Headers.Remove("Content-Type");
                                        queryString.Headers.Add("Content-Type", "application/json");
                                        response = await client.PostAsync(uri, queryString);

                                        if (failure)
                                        {
                                            Response.StatusCode = 500;
                                            output.Result = "INCOMPLETE_PROCESS";
                                        }
                                        else
                                        {
                                            output.Result = "OK";
                                        }
                                    }
                                }
                                else
                                {
                                    Response.StatusCode = 500;
                                    output.Result = "INTERNAL_ERROR";
                                }

                            }
                            else
                            {
                                Response.StatusCode = 400;
                                output.Result = "USER_IMAGESTORE_LESS_MIN";
                            }
                        }
                    }
                    else
                    {
                        Response.StatusCode = 400;
                        output.Result = "NO_PRIVILEGE";
                    }
                }
            }

            return output;
        }
    }
}
