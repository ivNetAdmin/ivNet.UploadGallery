﻿
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using Orchard.Mvc.Routes;

namespace ivNet.UploadGallery
{
    public class Routes : IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            var rdl = new List<RouteDescriptor>();
            rdl.AddRange(SiteRoutes());
            return rdl;
        }

        #region site
        private IEnumerable<RouteDescriptor> SiteRoutes()
        {
            return new[]
            {
                new RouteDescriptor
                {
                    Route = new Route(
                        "club/admin/image-upload",
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.UploadGallery"},
                            {"controller", "Gallery"},
                            {"action", "Upload"}
                        },
                        new RouteValueDictionary(),
                        new RouteValueDictionary
                        {
                            {"area", "ivNet.UploadGallery"}
                        },
                        new MvcRouteHandler())
                }
            };
        }
        #endregion
    }
}