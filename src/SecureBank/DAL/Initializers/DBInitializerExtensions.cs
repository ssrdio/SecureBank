using SecureBank.DAL.DBModels;
using SecureBank.Models.User;
using CommonUtils;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using SecureBank.Interfaces;
using System;
using System.Linq;

namespace SecureBank.DAL.Initializers
{
    public static class DbInitializerExtensions
    {
        public static void CreateDatabase(this IApplicationBuilder app)
        {
            ILogger logger = LogManager.GetCurrentClassLogger();

            using IServiceScope scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            IServiceProvider services = scope.ServiceProvider;

            try
            {
                IDbInitializer dbInitializer = services.GetRequiredService<IDbInitializer>();

                dbInitializer.Create(app);
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
                return;
            }
        }

        public static void InitializeDatabase(this IApplicationBuilder app, string admin, string adminPassword)
        {
            ILogger logger = LogManager.GetCurrentClassLogger();

            using IServiceScope scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            IServiceProvider services = scope.ServiceProvider;

            try
            {
                IDbInitializer dbInitializer = services.GetRequiredService<IDbInitializer>();

                dbInitializer.Initialize(admin, adminPassword);
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
                return;
            }
        }

        /// <summary>
        /// This adds admin user and some seed users with random transaction
        /// UserNames:
        /// carson.alexander@ssrd.io
        /// merdith.alonso@ssrd.io
        /// arturo.anad@ssrd.io
        /// gytis.barzdukas@ssrd.io
        /// yan.li@ssrd.io
        /// peggy.justice@ssrd.io
        /// laura.norman@ssrd.io
        /// nino.olivetto@ssrd.io
        /// </summary>
        public static void SeedDatabase(this IApplicationBuilder app, string userPassword)
        {
            ILogger logger = LogManager.GetCurrentClassLogger();

            using IServiceScope scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            IServiceProvider services = scope.ServiceProvider;

            try
            {
                IDbInitializer dbInitializer = services.GetRequiredService<IDbInitializer>();

                dbInitializer.Seed(userPassword);
            }
            catch (Exception ex)
            {
                logger.Error(ex, ex.Message);
                return;
            }
        }
    }
}
