using System.Collections.Generic;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Headers;
using System.Web.Http;
using Newtonsoft.Json;

namespace CRMHalalBackEnd
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Filters.Add(new AuthorizeAttribute());
            //   config.EnableCors();
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{lang}/{id}/{action_name}/{page_size}/{lastId}/",
                defaults: new
                {
                    lang = "az",
                    id = RouteParameter.Optional,
                    action_name = RouteParameter.Optional,
                    page_size = RouteParameter.Optional,
                    lastId = RouteParameter.Optional
                }
            );

            config.Routes.MapHttpRoute(
                name: "FilterApi",
                routeTemplate: "filter/{controller}/{lang}/{filter}/{sort}/{page_size}/{page}",
                defaults: new
                {
                    lang = "az",
                    pageSize = RouteParameter.Optional,
                    page = 1,
                    filter = RouteParameter.Optional,
                    sortingType = RouteParameter.Optional
                }
            );
            config.Routes.MapHttpRoute(
                name: "SlugApi",
                routeTemplate: "slug/{controller}/{action}/{lang}/{slug}",
                defaults: new { lang = "az", slug = "" }
            );
            config.Routes.MapHttpRoute(
                name: "SlugApi2",
                routeTemplate: "slug/{controller}/{action}/{lang}/{slug}",
                defaults: new { lang = "az", slug = "" }
            );
            config.Routes.MapHttpRoute(
                name: "NoteApi",
                routeTemplate: "note/api/{controller}/{action}/{lang}/{id}",
                defaults: new { lang = RouteParameter.Optional, id = RouteParameter.Optional }
            );

            
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("multipart/form-data"));
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

        }
    }
}