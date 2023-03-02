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
using SecureBank.Authorization;
using System.IO;
using Microsoft.Extensions.FileProviders;
using SecureBank.Filters;

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
                    options.IgnoreEmails = string.IsNullOrEmpty(appSettings.SmtpCredentials.Ip);
                    options.BaseUrl = appSettings.BaseUrl;
                });
            }
            else 
            {
                throw new Exception("Appsettings cannot be null");
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

            services.AddSingleton<ICookieService, CookieService>();

            CtfOptions ctfOptions = null;

            if (appSettings?.Ctf?.Enabled ?? false)
            {
                ctfOptions = services.ConfigureCtf(Configuration);
            }
            else 
            {
                ctfOptions = services.ConfigureWithoutCtf(Configuration);
            }

            services.AddControllersWithViews(options => 
            {
                options.Filters.Add(new GlobalExceptionFilter());
            });

            if (ctfOptions.CtfChallengeOptions.DirectoryBrowsing)
            {
                services.AddDirectoryBrowser();
            }

            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "BankWeb API", Version = "v1" });

                string xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml");
                x.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AppSettings appSettings = Configuration.GetSection("AppSettings").Get<AppSettings>();

            CtfOptions ctfOptions = app.ApplicationServices.GetRequiredService<IOptions<CtfOptions>>().Value;

            app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "SecureBank API");

                if (ctfOptions.CtfChallengeOptions.Swagger)
                {
                    CtfChallengeModel swaggerChallenge = ctfOptions.CtfChallenges
                        .Where(x => x.Type == CtfChallengeTypes.Swagger)
                        .Single();

                    string swaggerCssPath = "css/swagger.css";
                    string css = @$"
                    .topbar-wrapper img[alt='Swagger UI'], .topbar-wrapper span {{
                        visibility: hidden;
                    }}

                    .topbar-wrapper .link:after {{
                        content: 'SecureBank';
                        /*flag: {swaggerChallenge.Flag}*/
                        visibility: visible;
                        display: block;
                        position: absolute;
                        padding: 15px;
                        background: -moz-linear-gradient(left, #1d3ede, #01e6f8);
                        -webkit-background-clip: text;
                        -webkit-text-fill-color: transparent;
                    }}";

                    string fullPath = Path.Combine(env.ContentRootPath, "wwwroot", swaggerCssPath);

                    File.WriteAllText(fullPath, css);

                    x.InjectStylesheet($"/{swaggerCssPath}");
                }
            });

            app.UseStaticFiles();

            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "Documents")),
                RequestPath = "/docs",
                EnableDirectoryBrowsing = true,
            });

            if (ctfOptions.IsCtfEnabled && ctfOptions.CtfChallengeOptions.DirectoryBrowsing)
            {
                CtfChallengeModel ftpChallenge = ctfOptions.CtfChallenges
                    .Where(x => x.Type == CtfChallengeTypes.DirectoryBrowsing)
                    .Single();

                string fullPath = Path.Combine(env.ContentRootPath, "Documents", SecureBankConstants.DIRECTORY_BROWSING_FILE_NAME);

                File.WriteAllText(fullPath, ftpChallenge.Flag + new string(Enumerable.Repeat(' ', 3245).ToArray()));
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(routes =>
            {
                routes.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.CreateDatabase();

            if (appSettings.Ctf?.GenerateCtfdExport ?? false)
            {
                // If you are generating CTFd export on machine that you will run ctf don't forget to delete generated zip or users will be able
                // to download this zip because of Path Traversal vulnerability
                app.GenerateCtfdExport($"{AppContext.BaseDirectory}/Ctf");
            }

            if (Configuration["SeedingSettings:Seed"]?.ToLower() == "true")
            {
                if (!string.IsNullOrEmpty(Configuration["SeedingSettings:UserPassword"]))
                {
                    string password = Configuration["SeedingSettings:UserPassword"];

                    app.SeedDatabase(password);
                }
                else
                {
                    throw new Exception("User password is empty");
                }
            }

            app.InitializeDatabase(Configuration["SeedingSettings:Admin"], Configuration["SeedingSettings:AdminPassword"]);
        }
    }
}
