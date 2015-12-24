
using System.Collections.Generic;

namespace ivNet.UploadGallery.Models
{
    public class GalleryViewModel
    {
        public GalleryViewModel()
        {
            Galleries=new List<Gallery>();
        }
        public List<Gallery> Galleries { get; set; }
    }
}