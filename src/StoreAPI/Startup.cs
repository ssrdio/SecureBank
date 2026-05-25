using System;
using System.Linq;
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
                string storeConnectionString = string.Format("Host={0};Port={1};Database={2};Username={3};Password={4}",
                  storeDbSettings.Server,
                  storeDbSettings.ServerPort,
                  storeDbSettings.Database,
                  storeDbSettings.UserId,
                  storeDbSettings.UserPass);
                services.AddDbContext<StoreContext>(options => options.UseNpgsql(storeConnectionString));
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

            var ctfSettings = new CtfSettings
            {
                Enabled = Configuration["Ctf:Enabled"]?.Equals("true", StringComparison.OrdinalIgnoreCase) ?? false,
                Seed = Configuration["Ctf:Seed"] ?? "",
                FlagFormat = Configuration["Ctf:FlagFormat"] ?? "CTF{{{0}}}"
            };

            if (ctfSettings.Enabled && !string.IsNullOrEmpty(ctfSettings.Seed))
            {
                int hash = GetDeterministicHashCode(ctfSettings.Seed + "ssrf");
                var random = new Random(hash);
                const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
                var flagValue = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());
                ctfSettings.GeneratedFlag = string.Format(ctfSettings.FlagFormat, flagValue);
            }

            services.AddSingleton(ctfSettings);
        }

        private static int GetDeterministicHashCode(string str)
        {
            unchecked
            {
                int hash1 = (5381 << 16) + 5381;
                int hash2 = hash1;

                for (int i = 0; i < str.Length; i += 2)
                {
                    hash1 = (hash1 << 5) + hash1 ^ str[i];
                    if (i == str.Length - 1)
                        break;
                    hash2 = (hash2 << 5) + hash2 ^ str[i + 1];
                }

                return hash1 + hash2 * 1566083941;
            }
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
