

using Orchard.UI.Resources;

namespace ivNet.UploadGallery
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest.DefineScript("AngularJS").SetUrl("ivNet/anjularJs/anjular.min.js").SetVersion("1.2.9").SetDependencies("jQueryUI");
            manifest.DefineScript("AngularJSResource").SetUrl("ivNet/anjularJs/angular-resource.min.js").SetVersion("1.2.9").SetDependencies("AngularJS");
            manifest.DefineScript("trNgGrid").SetUrl("ivNet/trNgGrid/trNgGrid.min.js").SetVersion("1.2.9").SetDependencies("AngularJSResource");

            manifest.DefineScript("ivNet.Upload")
                .SetUrl("upload.min.js")
                .SetVersion("1.0")
                .SetDependencies("jQueryUI");

            manifest.DefineScript("ivNet.Gallery")
                .SetUrl("gallery.min.js")
                .SetVersion("1.0")
                .SetDependencies("trNgGrid");

            manifest.DefineScript("ivNet.GalleryCarousel")
          .SetUrl("gallery.carousel.min.js")
          .SetVersion("1.0")
          .SetDependencies("jQuery");
        }
    }
}