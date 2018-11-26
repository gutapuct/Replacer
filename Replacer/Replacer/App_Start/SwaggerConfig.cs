using System.Web.Http;
using WebActivatorEx;
using Replacer;
using Swashbuckle.Application;
using System;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace Replacer
{
    public class SwaggerConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config
                 .EnableSwagger(c => c.SingleApiVersion("v1", "Replacer"))
                 .EnableSwaggerUi();
        }
    }
}