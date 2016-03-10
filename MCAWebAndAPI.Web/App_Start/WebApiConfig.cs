﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Routing;

namespace MCAWebAndAPI.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var cors = new EnableCorsAttribute("http://103.28.56.18", "*", "*");
            config.EnableCors(cors);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                 name: "DefaultApi",
                 routeTemplate: "api/{controller}/{action}/{id}/{format}",
                 defaults: new { id = RouteParameter.Optional, format = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "Api_Post",
                routeTemplate: "{controller}/{id}/{action}",
                defaults: new { id = RouteParameter.Optional, action = "Post" },
                constraints: new { httpMethod = new HttpMethodConstraint(System.Net.Http.HttpMethod.Post) }
            );

            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);


        }
    }
}
