
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ivNet.UploadGallery.Services;

namespace ivNet.UploadGallery.Controllers.api
{
    public class ListController : ApiController
    {        
        private IGalleryServices _galleryServices;

        public ListController(IGalleryServices galleryServices)
        {
            _galleryServices = galleryServices;
        }
        
        [HttpGet]
        public HttpResponseMessage Get()
        {
            return Request.CreateResponse(HttpStatusCode.OK, _galleryServices.GetGalleryViewModel());
        }

        [HttpGet]
        public HttpResponseMessage Get(string id)
        {
            return Request.CreateResponse(HttpStatusCode.OK, _galleryServices.GetCarouselImages(id));
        }
    }
}