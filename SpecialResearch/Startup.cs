using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SpecialResearch.Data;
using System.Security.Claims;

namespace SpecialResearch
{
   

    public class Startup
    {
        public const string AdminRole = "admin";
        public const string RecieverRole = "receiver";
        public const string TesterRole = "tester";
        public const string ControllerRole = "controller";
        public const string ManagerRole = "manager";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Ёто сохранение доступа в кукисах
            services.AddAuthentication("Cookie")
                .AddCookie("Cookie",config=>
                {
                    config.LoginPath = "/Users/Login";
                    config.AccessDeniedPath = "/Users/AccessDenied";
                });

            //Ёто роли доступа. через клаймы
            services.AddAuthorization(options =>
            {
                options.AddPolicy("admin", builder =>
                {
                    builder.RequireClaim(ClaimTypes.Role, AdminRole);
                });
                options.AddPolicy("receiver", builder =>
                {
                    builder.RequireClaim(ClaimTypes.Role, RecieverRole);
                });
                options.AddPolicy("tester", builder =>
                {
                    builder.RequireClaim(ClaimTypes.Role, TesterRole);
                });
                options.AddPolicy("controller", builder =>
                {
                    builder.RequireClaim(ClaimTypes.Role, ControllerRole);
                });
                options.AddPolicy("manager", builder =>
                {
                    builder.RequireClaim(ClaimTypes.Role, ManagerRole);
                });
            });

            services.AddControllersWithViews();

            services.AddDbContext<SpecialResearchContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("SpecialResearchContext")));
            services.AddDistributedMemoryCache();
            services.AddSession( options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(60);//через час разлогинитьс€.
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            //ѕодключаем авторизацию
            app.UseAuthentication();
            app.UseAuthorization();
           

            app.UseSession();//—ессии больше не использую, но пусть будут.
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

           
        }
    }
}
