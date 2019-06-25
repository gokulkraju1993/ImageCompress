using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageProcessor;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net;

namespace ImageCompress.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        //[FromForm] string imagePath
        public ActionResult ImageCompress(string url)
        {   
            WebClient client = new WebClient();
            MemoryStream outStream11 = new MemoryStream();

            byte[] imageBytes = client.DownloadData(new Uri(url));
            List<byte[]> test = new List<byte[]>();
            var ss = Directory.GetCurrentDirectory();
            var tempFolderName = Guid.NewGuid().ToString();
            var tempFolder = Path.Combine(@"C:\Users\gokul.raju\requests", tempFolderName);

            string[] filePaths = new string[1] { "url" };
            string localFilePath = @"D:\Images";
            try
            {
                for (int i = 0; i < filePaths.Length; i++)
                {
                    //byte[] photoBytes = System.IO.File.ReadAllBytes(filePaths[i]);
                    byte[] photoBytes = imageBytes;
                    int quality = 40;
                    //string fileName = filePaths[i].Split('\\').Last();

                    string fileName = "Test.jpg";
                    //string filePath = filePaths[i].Replace("\\" + fileName, "");


                    Stream stream = new MemoryStream(photoBytes);

                    //FileStream file = stream;
                    var image = System.Drawing.Image.FromStream(stream);

                    var ratioX = (double)807 / image.Width;
                    var ratioY = (double)605 / image.Height;
                    var ratio = Math.Min(ratioX, ratioY);

                    var newWidth = (int)(image.Width * ratio);
                    var newHeight = (int)(image.Height * ratio);

                    var size = new Size(newWidth, newHeight);
                    if (stream.Length / 1024 < 150)
                    {
                        quality = 70;
                    }

                    
                    using (var inStream = new MemoryStream(photoBytes))
                    {
                        using (var outStream = new MemoryStream())
                        {
                            using (var imageFactory = new ImageFactory(preserveExifData: true))
                            {
                                imageFactory.Load(inStream)
                                    .Resize(size)
                                    .Quality(quality)
                                    
                                    .Save(outStream11);
                            }
                        }
                    }
                    stream.Dispose();
                    image.Dispose();
                    //GC.Collect();
                    //System.IO.File.Delete(filePaths[i]);
                    test.Add(outStream11.ToArray());
                 
                }
                GC.Collect();

            }
            catch (Exception eX)
            {
              
            }

            Stream myStream = outStream11;

            return File(myStream, "application/jpg", "Image.jpg");

        }

        public void HtmlToPdf(string url)
        {
            WebClient wc = new WebClient();

            wc.DownloadFile("http://localhost:91/convert?auth=arachnys-weaver&url=" + url, @"D:\users\somefile.pdf");
        }       

    }
    
}
