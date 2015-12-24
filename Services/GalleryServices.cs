

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using ivNet.UploadGallery.Models;
using Orchard;
using Orchard.Security;

namespace ivNet.UploadGallery.Services
{
    public interface IGalleryServices : IDependency
    {
        GalleryViewModel GetGalleryViewModel();
        List<CarouselImageViewModel> GetCarouselImages(string id);
    }

    public class GalleryServices : BaseService, IGalleryServices
    {
        public GalleryServices(IAuthenticationService authenticationService) : base(authenticationService)
        {
        }

        public GalleryViewModel GetGalleryViewModel()
        {
            var model = new GalleryViewModel();
            var directories =
                Directory.GetDirectories(System.Web.HttpContext.Current.Server.MapPath("~/Media/Default/Upload/Gallery"));
            var galleryConfigXml = new XmlDocument();
            foreach (var directory in directories)
            {
                var galleryConfigFile = Path.Combine(directory, GalleryConfigFile);

                galleryConfigXml.Load(galleryConfigFile);
                if (CurrentUser==null || galleryConfigXml.DocumentElement.SelectSingleNode("owner").InnerText == CurrentUser.UserName
                    || galleryConfigXml.DocumentElement.SelectSingleNode("type").InnerText == "Shared")
                {
                    var folderName = directory.Substring(directory.LastIndexOf('\\') + 1);
                    model.Galleries.Add(new Gallery
                    {
                        ImageCount = Directory.GetFiles(directory).ToList().Count() - 1,
                        Name = folderName,
                        Url = string.Format("/Media/Default/Upload/Gallery/{0}", folderName),
                        ImageUrl = GetGalleryThumbnail(directory, folderName)
                    });
                }
            }

            return model;
        }

        public List<CarouselImageViewModel> GetCarouselImages(string id)
        {            
            var rtnList = new List<CarouselImageViewModel>();

            if (id == "random") id = GetRandomGallery();

            var directory = System.Web.HttpContext.Current.Server.MapPath("~/Media/Default/Upload/Gallery/" + id);
    
            var galleryConfigFile = Path.Combine(directory, GalleryConfigFile);
            var galleryConfigXml = new XmlDocument();
            galleryConfigXml.Load(galleryConfigFile);

            var files = Directory.GetFiles(directory);
            foreach (var file in files)
            {
                if (!file.Contains(GalleryConfigFile))
                {
                    var fileName = file.Substring(file.LastIndexOf('\\') + 1);
                    var galleryDescription = galleryConfigXml.DocumentElement.SelectSingleNode("description").InnerText;
                    var metaDataNode =
                        galleryConfigXml.DocumentElement.SelectSingleNode(string.Format("image[name='{0}']", fileName));
                    rtnList.Add(new CarouselImageViewModel
                    {
                        Gallery = string.IsNullOrEmpty(galleryDescription) ? id : galleryDescription,
                        Url = string.Format("/Media/Default/Upload/Gallery/{0}/{1}", id, fileName),
                        Description = metaDataNode.SelectSingleNode("description").InnerText,
                        Photographer = metaDataNode.SelectSingleNode("photograper").InnerText
                        //Plot = metaDataNode.SelectSingleNode("plot").InnerText
                    });
                }
            }
            return rtnList;
        }

        private string GetRandomGallery()
        {
            var directories =
                 Directory.GetDirectories(System.Web.HttpContext.Current.Server.MapPath("~/Media/Default/Upload/Gallery"));

            var r = new Random();

            var rInt = r.Next(0, directories.Length);

            return directories[rInt].Substring(directories[rInt].LastIndexOf('\\') + 1);
        }

        private string GetGalleryThumbnail(string directory, string folderName)
        {
            var thumbNail = string.Format("/Media/Default/Upload/Gallery/{0}", "blank.png");
            var files = Directory.GetFiles(directory);
            foreach (var file in files)
            {
                if (!file.Contains(GalleryConfigFile))
                {
                    var fileName = file.Substring(file.LastIndexOf('\\') + 1);
                    thumbNail = string.Format("/Media/Default/Upload/Gallery/{0}/{1}", folderName, fileName); 
                    break;
                }
            }

            return thumbNail;
        }
    }
}