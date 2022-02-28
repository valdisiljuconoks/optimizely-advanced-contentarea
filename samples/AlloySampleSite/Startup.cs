using System.Collections.Generic;
using System.Globalization;
using AlloySampleSite.Extensions;
using AlloySampleSite.Infrastructure;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Data;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using EPiBootstrapArea.Providers;
using EPiServer.Authorization;
using EPiServer.Framework.Localization;
using EPiServer.Web;

namespace AlloySampleSite
{
    public class Startup
    {
        private readonly IWebHostEnvironment _webHostingEnvironment;
        private readonly IConfiguration _configuration;

        public Startup(IWebHostEnvironment webHostingEnvironment, IConfiguration configuration)
        {
            _webHostingEnvironment = webHostingEnvironment;
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var dbPath = Path.Combine(_webHostingEnvironment.ContentRootPath, "App_Data\\Alloy.mdf");
            var connectionstring = _configuration.GetConnectionString("EPiServerDB") ?? $"Data Source=(LocalDb)\\MSSQLLocalDB;AttachDbFilename={dbPath};Initial Catalog=alloy_mvc_netcore;Integrated Security=True;Connect Timeout=30;MultipleActiveResultSets=True";

            services.Configure<DataAccessOptions>(o =>
            {
                o.SetConnectionString(connectionstring);
            });

            services.AddCmsAspNetIdentity<ApplicationUser>(o =>
            {
                if (string.IsNullOrEmpty(o.ConnectionStringOptions?.ConnectionString))
                {
                    o.ConnectionStringOptions = new ConnectionStringOptions()
                    {
                        ConnectionString = connectionstring
                    };
                }
            });

            services.AddMvc();

            // add Episerver stuff
            var supportedCultures = new List<CultureInfo> { new("lv-LV"), new("sv"), new("no"), new("en") };

            services.AddCms()
                    .AddAlloy()
                    .AddEmbeddedLocalization<Startup>()
                    .Configure<LocalizationOptions>(o =>
                    {
                        o.FallbackBehavior = FallbackBehaviors.FallbackCulture;
                    })
                    .Configure<UIOptions>(uiOptions =>
                    {
                        uiOptions.UIShowGlobalizationUserInterface = true;
                    })
                    .Configure<RequestLocalizationOptions>(opts =>
                    {
                        //opts.SupportedCultures = supportedCultures;
                        //opts.SupportedUICultures = supportedCultures;
                        opts.ApplyCurrentCultureToResponseHeaders = true;
                    });

            services.AddBootstrapAreaRenderer(new DisplayModeFallbackDefaultProvider().GetAll);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMiddleware<AdministratorRegistrationPageMiddleware>();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapContent();
                endpoints.MapControllerRoute("Register", "/Register", new { controller = "Register", action = "Index" });
                endpoints.MapRazorPages();
            });
        }
    }
}
