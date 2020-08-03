using System.Web.Http;

namespace aas.web.api.classic
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "query/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}