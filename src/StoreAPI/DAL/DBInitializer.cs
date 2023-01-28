using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StoreAPI.DAL.DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StoreAPI.DAL
{
    public static class DBInitializer
    {
        public static void CreateDatabase(this IApplicationBuilder app)
        {
            StoreContext context;

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    context = services.GetRequiredService<StoreContext>();

                    context.Database.EnsureCreated();
                }
                catch (Exception ex)
                {
                    ILoggerFactory loggerFactory = services.GetRequiredService<ILoggerFactory>();
                    ILogger logger = loggerFactory.CreateLogger(typeof(DBInitializer));

                    logger.LogError(ex, "An error occurred while seeding the database.");
                    return;
                }
            }
        }

        public static void InitializeDatabase(this IApplicationBuilder app)
        {
            StoreContext context;

            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;


                ILoggerFactory loggerFactory = services.GetRequiredService<ILoggerFactory>();
                ILogger logger = loggerFactory.CreateLogger(typeof(DBInitializer));

                try
                {
                    context = services.GetRequiredService<StoreContext>();

                    context.Database.EnsureCreated();

                    if (context.StoreItems.Any())
                    {
                        logger.LogInformation($"Database is not empty");
                        return;   // DB has been seeded
                    }

                    logger.LogInformation($"Seeding store database");

                    string[] descriptions = new string[]
                    {
                        "Lorem ipsum dolor sit amet, consectetur adipiscing elit.",
                        "Nullam ut magna nec orci luctus tempus.",
                        "Donec nec leo feugiat, venenatis nisl vitae, varius urna.",
                        "Aenean a ipsum suscipit, mollis odio vitae, tincidunt elit.",
                        "Etiam ut mauris sed neque interdum elementum quis quis nibh.",
                        "Vivamus viverra mauris sit amet augue convallis ullamcorper.",
                        "Nunc bibendum velit a nisi convallis, quis tincidunt dui imperdiet.",
                        "Vestibulum elementum ex at nulla lobortis facilisis.",
                        "Sed sit amet quam eget tellus efficitur sodales.",
                        "Fusce nec risus at augue semper pharetra ut at leo.",
                        "Vivamus in dolor nec nibh malesuada blandit.",
                        "Aliquam lacinia quam lacinia venenatis rhoncus.",
                        "Pellentesque rutrum nisi eleifend, interdum nibh ac, varius nisl.",
                        "Suspendisse at lectus vehicula, consectetur nulla at, vestibulum erat.",
                        "Nullam in dui et ex lacinia lobortis a nec tellus."
                    };

                    string[] itemNames = new string[]
                    {
                        "1 month credit card froud protection",
                        "3 months credit card froud protection",
                        "1 year credit card froud protection",
                        "1 month insurance",
                        "3 moths insurance",
                        "1 year insurance",
                        "1 day vault",
                        "1 week vault",
                        "1 month vault",
                        "3 months vault",
                        "1 year vault",
                        "platinum card",
                        "mastercard",
                        "prepaid card",
                        "donation"
                    };

                    Random r = new Random();
                    for (int i = 0; i < 15; i++)
                    {
                        context.StoreItems.Add(new StoreItemTable
                        {
                            Name = itemNames[i],
                            Description = descriptions[i],
                            Price = r.Next(10, 150),
                            Installments = r.Next(1, 24)
                        });
                    }

                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while seeding the database.");
                    return;
                }
            }
        }
    }
}
