using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace basic_api
{
    public static class WebApiConfig
    {
		public static void Register(HttpConfiguration config)
		{
			// Web API configuration and services
			var cors = new EnableCorsAttribute("*", "*", "*");
			config.EnableCors(cors);

			// Web API routes
			config.MapHttpAttributeRoutes();

			config.Routes.MapHttpRoute(
					name: "DefaultApi",
					routeTemplate: "{controller}/{Action}/{id}",
					defaults: new { id = RouteParameter.Optional }
				);

			config.Formatters.JsonFormatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
			config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
		}
	}
}
