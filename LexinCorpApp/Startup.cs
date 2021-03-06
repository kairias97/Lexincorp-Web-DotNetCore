﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LexincorpApp.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using LexincorpApp.Models.ExternalServices;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Logging;
using Serilog;
using System.IO;
using Serilog.Events;
using Serilog.Context;

namespace LexincorpApp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            //  var builder = new ConfigurationBuilder()
            //.SetBasePath(env.ContentRootPath)
            //.AddJsonFile("appsettings.json",
            //             optional: false,
            //             reloadOnChange: true)
            //.AddEnvironmentVariables();

            //  if (env.IsDevelopment())
            //  {
            //      builder.AddUserSecrets<Startup>();
            //  }

            //  Configuration = builder.Build();
            this.Configuration = configuration;

        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseLazyLoadingProxies()
                    .UseSqlServer(Configuration["LexincorpAdmin:ConnectionString"]);
            });
            services.AddMvc()
                .AddJsonOptions(
                    options =>
                    {
                        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                        //options.SerializerSettings.DateFormatString = "dd/MM/yyyy";
                    }
                     );
            services.Configure<IISOptions>(options =>
            {
                options.ForwardClientCertificate = false;
                options.AutomaticAuthentication = false;
            });
            services.AddTransient<IClientTypeRepository, EFClientTypeRepository>();
            services.AddTransient<IBillingModeRepository, EFBillingModeRepository>();
            services.AddTransient<IDocumentDeliveryMethodRepository, EFDocumentDeliveryMethodRepository>();
            services.AddTransient<IClientRepository, EFClientRepository>();
            services.AddTransient<IDepartmentRepository, EFDepartmentRepository>();
            services.AddTransient<IUserRepository, EFUserRepository>();
            services.AddTransient<IAttorneyRepository, EFAttorneyRepository>();
            services.AddTransient<IExpenseRepository, EFExpenseRepository>();
            services.AddTransient<IVacationsMovementRepository, EFVacationsMovementRepository>();
            services.AddSingleton<ICryptoManager, BCryptManager>();
            services.AddSingleton<IGuidManager, GuidManager>();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
            services.AddAuthorization(options => {
                options.AddPolicy("CanPreBill", policy => policy.RequireClaim(PermissionsEnum.CanPreBill.ToString(), "true"));
                options.AddPolicy("CanBill", policy => policy.RequireClaim(PermissionsEnum.CanBill.ToString(), "true"));
                options.AddPolicy("CanAdminDeposits", policy => policy.RequireClaim(PermissionsEnum.CanAdminDeposits.ToString(), "true"));
                options.AddPolicy("CanReviewBillDetail", policy => policy.RequireClaim(PermissionsEnum.CanReviewBillDetail.ToString(), "true"));
                options.AddPolicy("CanApproveVacations", policy => policy.RequireClaim(PermissionsEnum.CanApproveVacations.ToString(), "true"));
            });
            services.AddTransient<IItemRepository, EFItemRepository>();
            services.AddTransient<IVacationsRequestRepository, EFVacationsRequestRepository>();
            services.AddTransient<ICategoryRepository, EFCategoryRepository>();
            services.AddTransient<IServiceRepository, EFServiceRepository>();
            services.AddTransient<IRetainerRepository, EFRetainerRepository>();
            services.AddSingleton<IMailSender, SendGridMailSender>();
            services.AddTransient<IPackageRepository, EFPackageRepository>();
            services.AddTransient<IRetainerSubscriptionRepository, EFRetainerSubscriptionRepository>();
            services.AddTransient<IActivityRepository, EFActivityRepository>();
            services.AddTransient<IBillableRetainerRepository, EFBillableRetainerRepository>();
            services.AddTransient<INotificationRepository, EFClosureNotificationRepository>();
            services.AddTransient<IBillRepository, EFBillRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MzA1NThAMzEzNjJlMzMyZTMwWTBCcXVjTHdxdDU2bU02Q2FBZzJRVXZwa0hqaUdjMkVZRWFNdmVNVFY1ND0=");
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            app.Use(async (httpContext, next) =>
            {
                var userName = httpContext.User.Identity.IsAuthenticated ? httpContext.User.Identity.Name : "anonymous";
                LogContext.PushProperty("User", !String.IsNullOrWhiteSpace(userName) ? userName : "unknown");
                await next.Invoke();
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStatusCodePages();
            app.UseStaticFiles();

            app.UseAuthentication();
            
            //Setup of culture of the app
            var defaultDateCulture = "es-NI";
            var ci = new CultureInfo(defaultDateCulture);
            ci.NumberFormat.NumberDecimalSeparator = ".";
            ci.NumberFormat.NumberGroupSeparator = ",";
            ci.NumberFormat.CurrencyDecimalSeparator = ".";

            // Configure the Localization middleware
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(ci),
                SupportedCultures = new List<CultureInfo>
                {
                    ci,
                },
                SupportedUICultures = new List<CultureInfo>
                {
                    ci,
                }
            });
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: null,
                    template: "",
                    defaults: new { controller = "Home", action = "Index" }
                    );
                routes.MapRoute(
                    name: null,
                    template: "{controller}/{action}/{id?}"
                    );
            });
            SeedData.EnsurePropulated(app);
            Log.Information("The app started successfully");
            
        }
    }
}
