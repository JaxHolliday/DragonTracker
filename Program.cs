using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonTracker.Data;
using DragonTracker.Helpers;
using DragonTracker.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using static System.Formats.Asn1.AsnWriter;


namespace DragonTracker
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            var host = CreateHostBuilder(args).Build();
            await DataHelper.ManageData(host);
            

            //using (var scope = host.Services.CreateScope())
            //{
            //    var services = scope.ServiceProvider;
            //    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            //    try
            //    {
            //        var context = services.GetRequiredService<ApplicationDbContext>();
            //        var userManager = services.GetRequiredService<UserManager<BTUser>>();
            //        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            //        await ContextSeed.SeedRolesAsync(roleManager);
            //        await ContextSeed.SeedDefaultUsersAsync(userManager);
            //        await ContextSeed.SeedDefaultTicketTypeAsync(context);
            //        await ContextSeed.SeedDefaultTicketStatusAsync(context);
            //        await ContextSeed.SeedDefaultTicketPriorityAsync(context);
            //        await ContextSeed.SeedProjectsAsync(context);
            //        await ContextSeed.SeedProjectUsersAsync(context, userManager);
            //        await ContextSeed.SeedTicketsAsync(context, userManager);
            //    }
            //    catch (Exception ex)
            //    {
            //        var logger = loggerFactory.CreateLogger<Program>();
            //        logger.LogError(ex, "An error occurred seeding the DB.");
            //    }
            //}
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.CaptureStartupErrors(true);
                    webBuilder.UseSetting(WebHostDefaults.DetailedErrorsKey, "true");
                    webBuilder.UseStartup<Startup>();
                });

        
    }
}
