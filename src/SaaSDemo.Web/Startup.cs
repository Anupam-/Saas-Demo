using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaaSDemo.Web.Data;
using SaaSDemo.Web.Services;
using SaaSDemo.Web.SaasKit;
using SaaSDemo.Identity;
using Microsoft.AspNetCore.Routing;
using SaaSDemo.Web.SaasKit.Middleware;
using SaaSDemo.Identity.Claims;

namespace SaaSDemo.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appInsights.json", optional: true, reloadOnChange: false)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();

                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: false);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMultitenancy<AppTenant, AppTenantDatabaseResolver>();

            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("SaaSDemo.Web")
                ));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddUserStore<TenantEnabledUserStore>()
                //.AddUserManager<TenantEnabledUserManager<ApplicationUser>>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();
            
            //add new Claims policies
            services.AddAuthorization(opt =>
            {
                //if the tenant is on at least basic, he can enter this.
                opt.AddPolicy("BasicServicePlan", policy => policy.RequireClaim("ServicePlan"));
                //tenants on prof can enter everything marked with Professionell
                opt.AddPolicy("ProfServicePlan", policy => policy.RequireClaim("ServicePlan", "Professionell"));
            });


            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<IClaimsTransformer, ServicePlanClaimsTransformer>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                //app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseMultitenancy<AppTenant>();

            var routeBuilder = new RouteBuilder(app);
            var routeTemplate = "tenant/{*filepath}";
            routeBuilder.MapRoute(routeTemplate, (IApplicationBuilder fork) =>
            {
                fork.UseMiddleware<TenantSpecificPathRewriteMiddleware>();
                fork.UseStaticFiles();
            });
            var router = routeBuilder.Build();
            app.UseRouter(router);

            app.UseStaticFiles();
            app.UseIdentity();
            //add claims transformation to read serviceplan
            app.UseClaimsTransformation(new ClaimsTransformationOptions
            {
                Transformer = new ServicePlanClaimsTransformer()
            });
            

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            
            SeedData.Initialize(app.ApplicationServices);
        }

    }
}
