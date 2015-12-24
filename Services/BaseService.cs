
using ivNet.UploadGallery.Helpers;
using Orchard.Security;

namespace ivNet.UploadGallery.Services
{
    public class BaseService
    {
        protected readonly IUser CurrentUser;
        protected readonly string GalleryConfigFile;

        public BaseService(IAuthenticationService authenticationService)
        {
            CurrentUser = authenticationService.GetAuthenticatedUser();
            GalleryConfigFile = ConfigHelper.GalleryConfigFile();
        } 
    }
}