
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Xml;
using ivNet.Club;
using ivNet.UploadGallery.Helpers;
using ivNet.UploadGallery.Models;
using ivNet.UploadGallery.Services;
using Orchard;
using Orchard.Localization;
using Orchard.Logging;
using Orchard.Security;
using Orchard.Themes;

namespace ivNet.UploadGallery.Controllers
{
    [Themed]
    public class GalleryController : Controller
    {
        private string _galleryConfigFile = ConfigHelper.GalleryConfigFile();
        private IGalleryServices _galleryServices;
        private readonly IOrchardServices _orchardServices;

        public GalleryController(

            IAuthenticationService authenticationService,
            IGalleryServices galleryServices,
            IOrchardServices orchardServices)
        {
            _galleryServices = galleryServices;
            _orchardServices = orchardServices;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            CurrentUser = authenticationService.GetAuthenticatedUser();
        }

        private IUser CurrentUser { get; set; }

        public Localizer T { get; set; }
        public ILogger Logger { get; set; }

        public ActionResult Upload()
        {
            if (!_orchardServices.Authorizer.Authorize(Permissions.ivGalleryTab, T("You are not authorized")))
                Response.Redirect("/Users/Account/AccessDenied?ReturnUrl=/club/admin/registration");

            return View(_galleryServices.GetGalleryViewModel());
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file, FormCollection data)
        {
            var galleryConfigXml = new XmlDocument();
            var filePath = string.Empty;

            if (!_orchardServices.Authorizer.Authorize(Permissions.ivGalleryTab, T("You are not authorized")))
                Response.Redirect("/Users/Account/AccessDenied?ReturnUrl=/club/admin/registration");

            if (!ModelState.IsValid) return View();

            if (file == null)
            {
                if (data["newGallery"] != null)
                {
                    filePath = GetFilePath(data["newGallery"]);
                    var galleryConfigFile = Path.Combine(filePath, _galleryConfigFile);
                    if (!Directory.Exists(filePath))
                    {
                        Directory.CreateDirectory(filePath);
                        
                        galleryConfigXml.LoadXml(
                            string.Format(
                                "<gallery><owner>{0}</owner><description>{1}</description></gallery>",
                                CurrentUser.UserName,
                                //data["galleryType"],
                                string.IsNullOrEmpty(data["galleryDescription"])
                                    ? string.Empty
                                    : data["galleryDescription"]));
                        galleryConfigXml.Save(galleryConfigFile);

                        ViewBag.Message = "Gallery created";
                    }
                    else
                    {
                        galleryConfigXml.Load(galleryConfigFile);
                        //if (galleryConfigXml.DocumentElement.SelectSingleNode("owner").InnerText == CurrentUser.UserName
                        //    || galleryConfigXml.DocumentElement.SelectSingleNode("type").InnerText == "Shared")
                        //{
                            ViewBag.Message = "Gallery already exists";
                        //}
                        //else
                        //{
                        //    ViewBag.Message = "Same personal gallery created by another user";
                        //}
                    }
                }                       
            }
            else if (file.ContentLength > 0)
            {
                const int maxContentLength = 1024 * 1024 * 4; //4 MB
                var allowedFileExtensions = new List<string> { ".jpg", ".png" }; //".gif" , ".pdf"

                if (!allowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.')).ToLowerInvariant()))
                {
                    ViewBag.Message = "Please file of type: " + string.Join(", ", allowedFileExtensions);
                }

                else if (file.ContentLength > maxContentLength)
                {
                    ViewBag.Message = "Your file is too large, maximum allowed size is: " + maxContentLength + " MB";
                }
                else
                {
                    var fileName = GetSafeFilename(Path.GetFileName(file.FileName)).Replace(" ",string.Empty);
                    filePath = GetFilePath(data["gallery"].Substring(0,data["gallery"].LastIndexOf('[')-1));
                                       
                    var path = Path.Combine(filePath, fileName);
                    ResizeSaveImage(file.InputStream, path);
             
                    // log data in XML file for that gallery                    
                    var galleryConfigFile = Path.Combine(filePath, _galleryConfigFile);

                    galleryConfigXml.Load(galleryConfigFile);
                   
                    var imageNode = galleryConfigXml.CreateNode("element", "image", "");
                    imageNode.InnerXml =
                        string.Format(
                            "<name>{0}</name>" +
                            "<photograper>{1}</photograper>" +
                            "<description>{2}</description>" +
                           // "<plot>{3}</plot>" +
                            "<uploadedBy>{3}</uploadedBy>" +
                            "<uploadedDate>{4} {5}</uploadedDate>",
                            fileName,
                            data["photograper"] ?? string.Empty,
                            data["description"] ?? string.Empty,
                           // data["plot"] ?? string.Empty, 
                            CurrentUser.UserName,
                            DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
       
                    galleryConfigXml.DocumentElement.AppendChild(imageNode);
                    galleryConfigXml.Save(galleryConfigFile);

                    //ModelState.Clear();
                    ViewBag.Message = "Image Uploaded";
                }
            }

            return View(_galleryServices.GetGalleryViewModel());
        }

     

        private string GetFilePath(string folder)
        {
            return Server.MapPath(
                string.Format("~/Media/Default/Upload/Gallery/{0}", GetSafeFilename(folder))
                    .Replace(" ", "_"));
        }

        private void ResizeSaveImage(Stream inputStream, string path)
        {
            double width = 0;
            double height = 0;
            var resizeHeight = 0;
            var resizeWidth = 0;
            var x = 0;
            var y = 0;
            
            GetImageSize(out width, out height);
            var ratio = width/height;

            var image = Image.FromStream(inputStream);

            // similar ratio           
            var imageRatio = Convert.ToDouble(image.Width) / Convert.ToDouble(image.Height) - ratio;
            if (imageRatio < 0) imageRatio = imageRatio * -1;
            if (imageRatio >= 0 && imageRatio <= 0.1)
            {
                resizeHeight = (int)height;
                resizeWidth = (int)width;
            }
            else
            {
                resizeWidth = (int) width;
                resizeHeight = (int)(image.Height*(width/image.Width));
                y = (int) ((resizeHeight - height)/2);
      
                if (resizeHeight < height)
                {
                    resizeHeight = (int) height;
                    resizeWidth = (int)(image.Width * (height / image.Height));
                    x = (int)((resizeWidth - width) / 2);
                    y = 0;
                }               
            }

            var resizedTempImageBitmap = new Bitmap(resizeWidth, resizeHeight);
            var resizedTempImageGraph = Graphics.FromImage(resizedTempImageBitmap);

            resizedTempImageGraph.CompositingQuality = CompositingQuality.HighQuality;
            resizedTempImageGraph.SmoothingMode = SmoothingMode.HighQuality;
            resizedTempImageGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

            resizedTempImageGraph.DrawImage(image, new Rectangle(0, 0, resizeWidth, resizeHeight));

            var resizedImageBitmap = new Bitmap((int)width, (int)height);
            var resizedImageGraph = Graphics.FromImage(resizedImageBitmap);

            resizedImageGraph.CompositingQuality = CompositingQuality.HighQuality;
            resizedImageGraph.SmoothingMode = SmoothingMode.HighQuality;
            resizedImageGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;

            resizedImageGraph.DrawImage(resizedTempImageBitmap, 
                new Rectangle(0, 0, (int)width, (int)height), 
                new Rectangle(x, y, (int)width, (int)height), GraphicsUnit.Pixel);

            resizedImageBitmap.Save(path, image.RawFormat);

            resizedTempImageBitmap.Dispose();
            resizedTempImageBitmap.Dispose();
            resizedImageBitmap.Dispose();
            resizedImageBitmap.Dispose();
            image.Dispose();
        }

        private void GetImageSize(out double width, out double height)
        {
            var fileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = HostingEnvironment.MapPath("~/Modules/ivNet.UploadGallery/web.config")
            };
            var configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            width = Convert.ToDouble(configuration.AppSettings.Settings["gallery.image.width"].Value);
            height = Convert.ToDouble(configuration.AppSettings.Settings["gallery.image.height"].Value);
        }

        private string GetSafeFilename(string filename)
        {
            return string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
        }
    }
}