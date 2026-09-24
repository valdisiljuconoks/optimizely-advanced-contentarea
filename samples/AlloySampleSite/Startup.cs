using System;
using System.Collections.Generic;
using AlloySampleSite.Extensions;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Data;
using EPiServer.DependencyInjection;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Linq;
using EPiServer.Core;
using EPiServer.Framework.Localization;
using EPiServer.Web;
using HtmlAgilityPack;
using TechFellow.Optimizely.AdvancedContentArea.Initialization;
using TechFellow.Optimizely.AdvancedContentArea.Providers;
using TechFellow.Optimizely.AdvancedContentArea;
using DisplayOptions = TechFellow.Optimizely.AdvancedContentArea.Providers.DisplayOptions;

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
            if (_webHostingEnvironment.IsDevelopment())
            {
                AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(_webHostingEnvironment.ContentRootPath, "App_Data"));
            }

            var connectionstring = _configuration.GetConnectionString("EPiServerDB");

            services.Configure<DataAccessOptions>(o =>
            {
                o.SetConnectionString(connectionstring);
                o.UpdateDatabaseCompatibilityLevel = true;
            });

            services.AddCmsAspNetIdentity<ApplicationUser>(o =>
            {
                if (string.IsNullOrEmpty(o.ConnectionStringOptions?.ConnectionString))
                {
                    o.ConnectionStringOptions = new ConnectionStringOptions
                    {
                        ConnectionString = connectionstring
                    };
                }
            });

            services
                .AddMvc()
                .AddViewLocalization();

            services.AddCms()
                    .AddAlloy()
                    .AddAdminUserRegistration()
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
                        opts.ApplyCurrentCultureToResponseHeaders = true;
                    });

            // EPiServer.Forms 6 (CMS 13) no longer self-registers - it has to be added explicitly
            services.AddForms();

            services.AddAdvancedContentArea(o =>
            {
                o.DisplayOptions = new List<DisplayModeFallback>(DisplayOptions.Default)
                {
                    new()
                    {
                        Id = "three-fifth",
                        Name = "Three fifth (3/5)",
                        Tag = "displaymode-three-fifth",
                        ExtraExtraLargeScreenWidth = 7,
                        ExtraExtraLargeScreenCssClassPattern = "col-three-fifth-xxl-{0}",
                        ExtraLargeScreenWidth = 7,
                        ExtraLargeScreenCssClassPattern = "col-three-fifth-xl-{0}",
                        LargeScreenWidth = 7,
                        LargeScreenCssClassPattern = "col-three-fifth-lg-{0}",
                        MediumScreenWidth = 12,
                        MediumScreenCssClassPattern = "col-three-fifth-md-{0}",
                        SmallScreenWidth = 12,
                        SmallScreenCssClassPattern = "col-three-fifth-sm-{0}",
                        ExtraSmallScreenWidth = 12,
                        ExtraSmallScreenCssClassPattern = "col-three-fifth-xs-{0}",
                        Icon = "epi-icon__layout--three-fifth"
                    },
                    DisplayModeFallback.None,
                };
                o.RowSupportEnabled = true;
                o.ItemStartRenderCallback = ItemStartRenderCallback;
            });
        }

        private void ItemStartRenderCallback(HtmlNode startTag, ContentAreaItem item, IContent content)
        {
            if (content.Name.Equals("AddCssClassViaCallbackBlock", StringComparison.CurrentCultureIgnoreCase))
            {
                startTag.AddClass("fancy-class-from-code");
                startTag.Attributes.Add("id", Guid.NewGuid().ToString());
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // required by Wangkanai.Detection (display channels)
            app.UseDetection();

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapContent();
                endpoints.MapRazorPages();
            });
        }
    }
}
