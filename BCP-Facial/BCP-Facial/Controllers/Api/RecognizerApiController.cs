using BCP_Facial.Data;
using BCP_Facial.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace BCP_Facial.Controllers.Api
{
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

        [HttpPost]
        [Route("Api/Recognizer/UploadImage")]
        public ImageUploadOutput UploadImage()
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
                        item.CopyToAsync(fs);
                        output.Url = _db.SiteConfigs.Where(e => e.Key.Equals("SITEURL")).FirstOrDefault().Value + "/recognizer/" + id + "." + ext[ext.Length - 1];
                    }
                }
            }
            output.Result = "OK";

            return output;
        }
    }
}
