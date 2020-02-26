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

            if (input == null)
            {
                Response.StatusCode = 400;
                output.Result = "INPUT_IS_NULL";
            } else
            {
                if (aspUser.IsAdmin)
                {
                    BCPUser user = _db._BCPUsers.Where(e => e.Id.Equals(input.StudentId)).FirstOrDefault();
                    
                    if (user == null)
                    {
                        Response.StatusCode = 400;
                        output.Result = "USER_NOT_FOUND";
                    } else
                    {
                        List<UserImage> imageStore = user.List_UserImage.Where(e => e.Status == 1).OrderBy(e => e.DateCreated).ToList();
                        int min = int.Parse(_db.SiteConfigs.Where(e => e.Key.Equals("NUM_PHOTO_PER_STUDENT")).FirstOrDefault().Value);
                        string siteUrl = _db.SiteConfigs.Where(e => e.Key.Equals("SITEURL")).FirstOrDefault().Value;
                        int imageCount = imageStore.Count;
                        string faceIdToCompare = null;
                        string personGroupId = _db.SiteConfigs.Where(e => e.Key.Equals("PERSONGROUP")).FirstOrDefault().Value;

                        if (imageCount >= min)
                        {
                            string faceListId = Guid.NewGuid().ToString().ToLower();
                            Uri uri = new Uri("https://bcp-facial.cognitiveservices.azure.com/face/v1.0/facelists/" + faceListId);
                            HttpClientHandler handler = new HttpClientHandler();
                            StringContent queryString = null;
                            HttpClient client = new HttpClient(handler);
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
                            HttpResponseMessage response = await client.PutAsync(uri, queryString);
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
                                            string respond = await response.Content.ReadAsStringAsync();

                                            dynamic jsonObj = JsonConvert.DeserializeObject(respond);
                                            item.FaceId = jsonObj.persistedFaceId;
                                        }

                                        _db.SaveChanges();
                                        imageCount--;
                                    } else
                                    {
                                        uri = new Uri("https://bcp-facial.cognitiveservices.azure.com/face/v1.0/detect");
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
                                            string respond = await response.Content.ReadAsStringAsync();

                                            dynamic jsonObj = JsonConvert.DeserializeObject(respond);
                                            item.Status = 0;
                                            faceIdToCompare = jsonObj.faceId;
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
                                    FaceListId = faceListId
                                };

                                queryString = new StringContent(JsonConvert.SerializeObject(pgInput), Encoding.UTF8, "application/json");
                                queryString.Headers.Remove("Content-Type");
                                queryString.Headers.Add("Content-Type", "application/json");
                                response = await client.PostAsync(uri, queryString);

                                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    string respond = await response.Content.ReadAsStringAsync();

                                    dynamic jsonObj = JsonConvert.DeserializeObject(respond);
                                    foreach (var item in jsonObj)
                                    {
                                        string id = item.persistedFaceId;
                                        UserImage ui = _db.UserImages.Where(e => e.FaceId.Equals(id)).FirstOrDefault();
                                        ui.Confidence = item.confidence;

                                        _db.SaveChanges();
                                    }
                                }

                                uri = new Uri("https://bcp-facial.cognitiveservices.azure.com/face/v1.0/facelists/"+faceListId);
                                handler = new HttpClientHandler();
                                queryString = null;
                                client = new HttpClient(handler);
                                //string respond = null;
                                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "8d4c42ecbb784d909275492115ea56f0");

                                queryString = new StringContent("", Encoding.UTF8, "application/json");
                                queryString.Headers.Remove("Content-Type");
                                queryString.Headers.Add("Content-Type", "application/json");
                                response = await client.DeleteAsync(uri);

                                if (user.PersonId != null)
                                {
                                    uri = new Uri("https://bcp-facial.cognitiveservices.azure.com/face/v1.0/persongroups/{personGroupId}/persons/"+user.PersonId);
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

                                uri = new Uri("https://bcp-facial.cognitiveservices.azure.com/face/v1.0/persongroups/"+ personGroupId +"/persons");
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
                                    string respond = await response.Content.ReadAsStringAsync();
                                    dynamic jsonObj = JsonConvert.DeserializeObject(respond);
                                    user.PersonId = jsonObj.personId;
                                    _db.SaveChanges();
                                }

                                foreach(UserImage item in imageStore)
                                {
                                    if (item.Status == 1)
                                    {
                                        uri = new Uri("https://bcp-facial.cognitiveservices.azure.com/face/v1.0/persongroups/"+personGroupId+"/persons/"+user.PersonId+"/persistedFaces");
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
                                            string respond = await response.Content.ReadAsStringAsync();
                                            dynamic jsonObj = JsonConvert.DeserializeObject(respond);
                                            item.FaceId = jsonObj.persistedFaceId;
                                            item.Status = 2;
                                            _db.SaveChanges();
                                        }
                                    }
                                }

                                output.Result = "OK";
                            } else
                            {
                                Response.StatusCode = 500;
                                output.Result = "INTERNAL_ERROR";
                            }
                            
                        } else
                        {
                            Response.StatusCode = 400;
                            output.Result = "USER_IMAGESTORE_LESS_MIN";
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
    }
}
