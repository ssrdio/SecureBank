using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StoreAPI.DAL;
using StoreAPI.Models;
using Microsoft.Extensions.Hosting;
using NLog.Fluent;

namespace StoreAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            DatabaseSettings storeDbSettings = Configuration.GetSection("DatabaseConnections:StoreMSSQL").Get<DatabaseSettings>();
            if (storeDbSettings != null && !String.IsNullOrEmpty(storeDbSettings.Database))
            {
                // configure mssql
                string storeConnectionString = string.Format("Server={0},{1};Database={2};User Id={3};Password={4};TrustServerCertificate=True",
                  storeDbSettings.Server,
                  storeDbSettings.ServerPort,
                  storeDbSettings.Database,
                  storeDbSettings.UserId,
                  storeDbSettings.UserPass);
                services.AddDbContext<StoreContext>(options => options.UseSqlServer(storeConnectionString));
            }
            else
            {
                //configure sqlite
                services.AddDbContext<StoreContext>(options => options.UseSqlite("Filename=./storeDB.db"));
            }

            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "StoreAPI", Version = "v1" });

                string xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml");
                x.IncludeXmlComments(xmlPath);
            });

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "StoreAPI v1");
            });

            app.UseRouting();

            app.CreateDatabase();

            if (Configuration["StoreAPI:Seed"].ToUpper() == "TRUE")
            {
                app.InitializeDatabase();
            }

            app.UseEndpoints(routes =>
            {
                routes.MapControllerRoute(
                    name: "default",
                    pattern: "/api/{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
