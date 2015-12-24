
using System.Configuration;
using System.Web.Hosting;

namespace ivNet.UploadGallery.Helpers
{
    public static class ConfigHelper
    {
        public static string GalleryConfigFile()
        {
            var fileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = HostingEnvironment.MapPath("~/Modules/ivNet.UploadGallery/web.config")
            };
            var configuration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            return configuration.AppSettings.Settings["gallery.config.file"].Value;
        }
    }
}