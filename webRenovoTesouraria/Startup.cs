using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using webRenovoTesouraria.AcessoDados;
using webRenovoTesouraria.AcessoDados.Repositories.Contracts;
using webRenovoTesouraria.Models;

namespace webRenovoTesouraria
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public static IConfiguration StaticConfig { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            StaticConfig = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
            //{
            //    options.LoginPath = "/Home/Index/";
            //    options.AccessDeniedPath = new PathString("/Home/Index/");
            //    options.ExpireTimeSpan = System.TimeSpan.FromMinutes(30);
            //});

            //var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            //services.AddMvc(config =>
            //{
            //    config.Filters.Add(new AuthorizeFilter(policy));                
            //}).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = System.TimeSpan.FromMinutes(90);
                //options.Cookie.HttpOnly = true;
                //options.Cookie.Name = "RenovoTesouraria.Session";
                //options.Cookie.IsEssential = true;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddDbContext<RenovoContext>(opt => opt.UseMySQL(Configuration.GetConnectionString("RenovoConn")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            CultureInfo.CurrentCulture = new CultureInfo("pt-br");

            app.UseDeveloperExceptionPage();
            app.UseStaticFiles();
            //app.UseAuthentication();
            app.UseSession();

            app.UseStatusCodePagesWithReExecute("/home/error/{0}");
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
