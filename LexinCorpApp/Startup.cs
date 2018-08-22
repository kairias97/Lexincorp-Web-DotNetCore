using System;
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

namespace LexincorpApp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseLazyLoadingProxies()
                    .UseSqlServer(Configuration["Data:LexincorpAdmin:ConnectionString"]);
            });
            services.AddMvc()
                .AddJsonOptions(
                    options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                );
            services.AddTransient<IClientTypeRepository, EFClientTypeRepository>();
            services.AddTransient<IBillingModeRepository, EFBillingModeRepository>();
            services.AddTransient<IDocumentDeliveryMethodRepository, EFDocumentDeliveryMethodRepository>();
            services.AddTransient<IClientRepository, EFClientRepository>();
            services.AddTransient<IDepartmentRepository, EFDepartmentRepository>();
            services.AddTransient<IUserRepository, EFUserRepository>();
            services.AddTransient<IAttorneyRepository, EFAttorneyRepository>();
            services.AddTransient<IExpenseRepository, EFExpenseRepository>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStatusCodePages();
            app.UseStaticFiles();
            
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

        }
    }
}
