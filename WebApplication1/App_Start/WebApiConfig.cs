using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Web.Http.Results;
using Newtonsoft.Json;

namespace WebApplication1
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API 配置和服务

            // Web API 路由
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            #region Gold Config
            var setting = new JsonSerializerSettings();
            JsonConvert.DefaultSettings = new Func<JsonSerializerSettings>(() =>
            {
                //日期类型默认格式化处理
                setting.DateFormatHandling = Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat;
                setting.DateFormatString = "yyyy-MM-dd HH:mm:ss";

                //空值处理
                setting.NullValueHandling = NullValueHandling.Ignore;

                return setting;
            });
            #endregion
        }
    }

    public static class ApiControllerExtension
    {
        private static JsonSerializerSettings GoldJsonSerializerSettings = new JsonSerializerSettings()
        {
            //日期类型默认格式化处理
            DateFormatHandling = Newtonsoft.Json.DateFormatHandling.MicrosoftDateFormat,
            DateFormatString = "yyyy-MM-dd HH:mm:ss",
            //空值处理
            NullValueHandling = NullValueHandling.Ignore
        };

        public static IHttpActionResult ToJson<T>(this ApiController controller, T module)
        {
            return new JsonResult<T>(module, GoldJsonSerializerSettings, Encoding.UTF8, controller);
        }
    }
}
