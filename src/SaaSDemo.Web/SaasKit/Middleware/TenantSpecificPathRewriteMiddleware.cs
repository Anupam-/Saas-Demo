using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using SaaSDemo.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SaaSDemo.Web.SaasKit.Middleware
{
    public class TenantSpecificPathRewriteMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly IHostingEnvironment _env;

        public TenantSpecificPathRewriteMiddleware(RequestDelegate next, ILoggerFactory loggerFactory, IHostingEnvironment env)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<TenantSpecificPathRewriteMiddleware>();
            _env = env;
        }

        public async Task Invoke(HttpContext context)
        {
            var tenantContext = context.GetTenantContext<AppTenant>();

            if (tenantContext != null)
            {
                var originalPath = context.Request.Path;
                //get the tenantFolder from the current tenant bject
                var tenantFolder = tenantContext.Tenant.Folder;
                //retrieve the filepath portion from the route. Defined in config. Must match
                var filePath = context.GetRouteValue("filepath");
                //build out the new string
                var newPath = new PathString($"/tenant/{tenantFolder}/{filePath}");
                //if the new file doesn't exist, fall back to core
                var f = _env.WebRootPath + newPath; 
                if (!File.Exists(f))
                {
                    newPath = new PathString($"/core/{filePath}");
                }
                context.Request.Path = newPath;
                _logger.LogDebug($"Tenant specific static file requested, path rewritten from {originalPath} to {newPath} ");
                await _next(context);

                context.Request.Path = originalPath;
            }

            


        }
    }
}
