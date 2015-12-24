
using System.Collections.Generic;
using Orchard.Mvc.Routes;
using Orchard.WebApi.Routes;

namespace ivNet.UploadGallery
{
    public class RoutesApi : IHttpRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            foreach (var routeDescriptor in GetRoutes())
                routes.Add(routeDescriptor);
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            var rdl = new List<RouteDescriptor>();
            rdl.AddRange(Routes());
            return rdl;
        }

        private IEnumerable<RouteDescriptor> Routes()
        {
            return new[]
            {
                new HttpRouteDescriptor
                {
                    RouteTemplate = "api/gallery/{controller}/{id}",
                    Defaults = new
                    {
                        area = "ivNet.UploadGallery",
                        id = System.Web.Http.RouteParameter.Optional
                    }
                },
            };
        }
    }
}