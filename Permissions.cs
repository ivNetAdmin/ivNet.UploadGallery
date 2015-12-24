
using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace ivNet.Club
{
    public class Permissions : IPermissionProvider
    {

        public static readonly Permission ivGalleryTab = new Permission
        {
            Description = "Access gallery website tab",
            Name = "ivGalleryTab"
        };


      
        public Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ivGalleryTab
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return null;
        }
    }
}