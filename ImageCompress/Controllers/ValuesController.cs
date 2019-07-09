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
using TinifyAPI;
using ImageMagick;

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
            catch (System.Exception eX)
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

        public void ImageMagick(double scaleFactor = 1)
        {
            //Tinify.Key = "0WgAiL9LFYVpIa8HLRDMdBBM4xNS5wzc";
            //Tinify.FromFile(@"D:\2\SampleJPGImage_15mbmb.jpg").ToFile(@"D:\2\SampleJPGImage_15mbmb__.jpg").Wait();

            //FileInfo snakewareLogo = new FileInfo(@"C:\Users\gokul.raju\Desktop\Z2.jpg");
            //File.Copy(@"C:\Users\gokul.raju\Desktop\Z2.jpg", "Z2.jpg", true);

            //Console.WriteLine("Bytes before: " + snakewareLogo.Length);

            //ImageOptimizer optimizer = new ImageOptimizer();
            //optimizer.Compress(snakewareLogo);

            //snakewareLogo.Refresh();
            //Console.WriteLine("Bytes after:  " + snakewareLogo.Length);
            string[] filePaths = Directory.GetFiles(@"D:\IMG", "*.jpg", SearchOption.AllDirectories);
            foreach (string path in filePaths)
            {
                WebClient client = new WebClient();
                byte[] imageBytes = System.IO.File.ReadAllBytes(path);
                Stream stream = new MemoryStream(imageBytes);
                string fileName = path.Split('\\').Last();

                string filePathC = path.Replace(fileName, "") + "\\C\\_" + fileName;
                string filePathR = path.Replace(fileName, "") + "\\R\\_" + fileName;
                //if (fileName.Substring(0, 1) != "_")
                //{

                try
                {
                    ReduceImageSize(scaleFactor, stream, filePathR);
                    compress(System.Drawing.Image.FromStream(stream), 50, filePathC);
                }
                catch {

                }
                //}
                //else{
                //    try
                //    {
                //        System.IO.File.Delete(path);
                //    }
                //    catch { }
                //}
            }
        }


        public void ReduceImageSize(double scaleFactor, Stream sourcePath, string targetPath)
        {
            try
            {
                using (var image = System.Drawing.Image.FromStream(sourcePath))
                {
                    var newWidth = (int)(image.Width * scaleFactor);
                    var newHeight = (int)(image.Height * scaleFactor);
                    var thumbnailImg = new Bitmap(newWidth, newHeight);
                    var thumbGraph = Graphics.FromImage(thumbnailImg);
                    thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                    thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                    thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    thumbGraph.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
                    thumbGraph.DrawImage(image, imageRectangle);
                    thumbnailImg.Save(targetPath, image.RawFormat);
                }
            }
            catch {
            }

        }

        public void compress(Image sourceImage, int imageQuality, string savePath)
        {
            ImageCodecInfo jpegCodec = null;

            //Set quality factor for compression
            EncoderParameter imageQualitysParameter = new EncoderParameter(
                        System.Drawing.Imaging.Encoder.Quality, imageQuality);

            //List all avaible codecs (system wide)
            ImageCodecInfo[] alleCodecs = ImageCodecInfo.GetImageEncoders();

            EncoderParameters codecParameter = new EncoderParameters(1);
            codecParameter.Param[0] = imageQualitysParameter;

            //Find and choose JPEG codec
            for (int i = 0; i < alleCodecs.Length; i++)
            {
                if (alleCodecs[i].MimeType == "image/jpeg")
                {
                    jpegCodec = alleCodecs[i];
                    break;
                }
            }

            //Save compressed image
            sourceImage.Save(savePath, jpegCodec, codecParameter);
        }




    }
    
}
