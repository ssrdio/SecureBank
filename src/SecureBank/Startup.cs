using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SecureBank.Services;
using NLog;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;
using SecureBank.Ctf;
using SecureBank.DAL.DAO;
using SecureBank.DAL.Initializers;
using SecureBank.DAL;
using SecureBank.Helpers.Authorization;
using SecureBank.Interfaces;
using SecureBank.Models;
using SecureBank.Ctf.Models;
using Microsoft.Extensions.Options;
using System.Linq;

namespace SecureBank
{
    public class Startup
    {
        internal static ILogger _logger = LogManager.GetCurrentClassLogger();

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            Configuration = configuration;
            Environment = webHostEnvironment;
        }

        public IConfiguration Configuration { get; }
        public IWebHostEnvironment Environment { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            AppSettings appSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();

            if (appSettings != null)
            {
                services.Configure<AppSettings>(options =>
                {
                    options.StoreEndpoint = appSettings.StoreEndpoint;
                    options.SmtpCredentials = appSettings.SmtpCredentials;
                    options.LegalURL = appSettings.LegalURL;
                    options.IgnoreEmails = appSettings.SmtpCredentials == null || string.IsNullOrEmpty(appSettings.SmtpCredentials.Ip) ? true : false;
                });
            }

            DatabaseSettings customerDbSettings = Configuration.GetSection("DatabaseConnections:SecureBankMSSQL").Get<DatabaseSettings>();
            if (customerDbSettings != null)
            {              
                string customerConnectionString = string.Format("Server={0},{1};Database={2};User Id={3};Password={4}",
                    customerDbSettings.Server,
                    customerDbSettings.ServerPort,
                    customerDbSettings.Database,
                    customerDbSettings.UserId,
                    customerDbSettings.UserPass);
                _logger.Info(customerConnectionString);
                // configure mssql
                services.AddDbContext<PortalDBContext>(options => options.UseSqlServer(customerConnectionString));
            }
            else
            {
                //configure sqlite
                services.AddDbContext<PortalDBContext>(options => options.UseSqlite("Filename=./customerDB.db"));
            }

            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();

            services.AddTransient<ITransactionDAO, TransactionDAO>();
            services.AddTransient<IUserDAO, UserDAO>();
            services.AddTransient<IDbInitializer, DbInitializer>();

            services.AddScoped<StoreAPICalls>();
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddScoped<IUserBL, UserBL>();
            services.AddScoped<ITransactionBL, TransactionBL>();
            services.AddScoped<IStoreBL, StoreBL>();
            services.AddScoped<IPortalSearchBL, PortalSearchBL>();
            services.AddScoped<IAuthBL, AuthBL>();
            services.AddScoped<IUploadFileBL, UploadFileBL>();
            services.AddScoped<IAdminBL, AdminBL>();
            services.AddScoped<IAdminStoreBL, AdminStoreBL>();

            services.AddAuthorization(options => 
            {
                AuthorizationPolicyBuilder defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder();
                defaultAuthorizationPolicyBuilder.RequireAssertion(x => true);

                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });

            services.AddSingleton<IAuthorizeService, AuthorizeService>();
            services.AddSingleton<IUserExtensions, UserExtensions>();

            if (appSettings?.Ctf?.Enabled ?? false)
            {
                services.ConfigureCtf(Configuration);
            }

            services.AddControllersWithViews();

            services.AddSwaggerGen(x =>
            {
#pragma warning disable ASP0000
                IServiceProvider serviceProvider = services.BuildServiceProvider();
                CtfOptions ctfOptions = serviceProvider.GetService<IOptions<CtfOptions>>()?.Value;

                CtfChallangeModel swaggerChallange = ctfOptions?.CtfChallanges
                    .Where(x => x.Type == CtfChallengeTypes.Swagger)
                    .Single();

                x.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "BankWeb API", Version = "v1", Description = swaggerChallange?.Flag });

                string xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml");
                x.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AppSettings appSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();

            app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "SecureBank API");
            });
            app.UseStaticFiles();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(routes =>
            {
                routes.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.CreateDatabase();

            if (appSettings?.Ctf?.Enabled ?? false)
            {
                // If you are generating CTFd export on mashine that you will run ctf don't forget to delete generated zip or users will be able
                // to download this zip becuse of Path Treversial vulnerability
                app.GenerateCtfdExport($"{AppContext.BaseDirectory}/Ctf");
            }

            //if (Configuration["SecureBank:Seed"] == "true")
            //{
            //    string password = "Password1!";
            //    if (!string.IsNullOrEmpty(Configuration["SecureBank:Seed:Password"]))
            //    {
            //        password = Configuration["SecureBank:Seed:Password"];
            //    }

            //    app.InitializeDatabase(Configuration["SecureBank:Seed:Admin"], Configuration["SecureBank:Seed:AdminPassword"], password);
            //}

            //app.InitializeDatabase("admin@testing.ssrd.io", "admin", "admin");
        }
    }
}
