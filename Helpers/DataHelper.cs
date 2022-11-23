using DragonTracker.Data;
using DragonTracker.Data.Enums;
using DragonTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DragonTracker.Helpers
{
    public class DataHelper
    {

        public static string GetConnectionString(IConfiguration configuration)
        {
            //Default connection string coming from appsettings like usual
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            //it will be automatically overwritten if running on heroku
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            return string.IsNullOrEmpty(databaseUrl) ? connectionString : BuildConnectionString(databaseUrl);
        }

        public static string BuildConnectionString(string databaseUrl)
        {
            //Provides an object representation of a uniform resource identifier (URI) and easy access to parts of the URI.
            var databaseUri = new Uri(databaseUrl);
            var userInfo = databaseUri.UserInfo.Split(':');

            //Provides a simple way to create and manage the contentson connection strings used by the NpgsqlConnection class
            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/'),
                SslMode = SslMode.Prefer,
                TrustServerCertificate = true
            };

            return builder.ToString();
        }

        public static async Task ManageData(IHost host)
        {
            try
            {
                using var svcScope = host.Services.CreateScope();
                var svcProvider = svcScope.ServiceProvider;

                var dbContextSvc = svcProvider.GetRequiredService<ApplicationDbContext>();
                var roleManagerSvc = svcProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManagerSvc = svcProvider.GetRequiredService<UserManager<BTUser>>();

                await dbContextSvc.Database.MigrateAsync();

                await SeedRolesAsync(roleManagerSvc);
                await SeedDefaultUsersAsync(userManagerSvc);
                await SeedDefaultTicketTypeAsync(dbContextSvc);
                await SeedDefaultTicketStatusAsync(dbContextSvc);
                await SeedDefaultTicketPriorityAsync(dbContextSvc);
                await SeedProjectsAsync(dbContextSvc);
                await SeedTicketsAsync(dbContextSvc, userManagerSvc);
            }

            catch (PostgresException ex)
            {
                Console.WriteLine($"Exception while running Manage Data => {ex}");
            }

        }

        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.ProjectManager.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Developer.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Submitter.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.NewUser.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Demo.ToString()));
        }

        //Seed Users
        public static async Task SeedDefaultUsersAsync(UserManager<BTUser> userManager)
        {
            #region adminuser
            //Seed Admin User
            var defaultUser = new BTUser
            {
                UserName = "hollidaycodes@gmail.com",
                Email = "hollidaycodes@gmail.com",
                FirstName = "Jackson",
                LastName = "Holliday",
                EmailConfirmed = true
            };
            try
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "Holliday1993!");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("******** ERROR **********");
                Debug.WriteLine("Error Seeding Default Admin User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("*************************");
                throw;
            }
            #endregion
            #region projectmanageruser
            //Seed Project Manager User
            var pmUser = new BTUser
            {
                UserName = "jaxholliday@gmail.com",
                Email = "jaxholliday@gmail.com",
                FirstName = "Andrew",
                LastName = "Holliday",
                EmailConfirmed = true
            };
            try
            {
                var user = await userManager.FindByEmailAsync(pmUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(pmUser, "Holliday1993!");
                    await userManager.AddToRoleAsync(pmUser, Roles.ProjectManager.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("******** ERROR **********");
                Debug.WriteLine("Error Seeding Default Project Manager User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("*************************");
                throw;
            }
            #endregion
            #region developer
            //Seed Developer User
            var devUser = new BTUser
            {
                UserName = "jeffholliday11@gmail.com",
                Email = "jeffholliday11@gmail.com",
                FirstName = "Jeff",
                LastName = "Holliday",
                EmailConfirmed = true
            };
            try
            {
                var user = await userManager.FindByEmailAsync(devUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(devUser, "Holliday1993!");
                    await userManager.AddToRoleAsync(devUser, Roles.Developer.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("******** ERROR **********");
                Debug.WriteLine("Error Seeding Developer Admin User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("*************************");
                throw;
            }
            #endregion
            #region submitter
            //Seed submitter User
            var subUser = new BTUser
            {
                UserName = "jaxhollidayau@gmail.com",
                Email = "jaxhollidayau@gmail.com",
                FirstName = "Aussie",
                LastName = "Outback",
                EmailConfirmed = true
            };
            try
            {
                var user = await userManager.FindByEmailAsync(subUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(subUser, "Holliday1993!");
                    await userManager.AddToRoleAsync(subUser, Roles.Submitter.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("******** ERROR **********");
                Debug.WriteLine("Error Seeding Default Submitter User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("*************************");
                throw;
            }
            #endregion
            #region newuser
            //Seed New User
            var NewUser = new BTUser
            {
                UserName = "NewUserhollidayau@gmail.com",
                Email = "NewUserjaxhollidayau@gmail.com",
                FirstName = "Bruh",
                LastName = "Outback",
                EmailConfirmed = true
            };
            try
            {
                var user = await userManager.FindByEmailAsync(subUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(subUser, "Holliday1993!");
                    await userManager.AddToRoleAsync(subUser, Roles.NewUser.ToString());
                    await userManager.AddToRoleAsync(subUser, Roles.Demo.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("******** ERROR **********");
                Debug.WriteLine("Error Seeding Default New User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("*************************");
                throw;
            }
            #endregion

            string demoPassword = "Abc&123!";
            //seeded demo users for showoff
            //each user occupies a main role
            //target demo role to prevent demo from changing database
            #region demoadminuser
            //Seed Admin User
            var demoAdminUser = new BTUser
            {
                UserName = "demohollidaycodes@gmail.com",
                Email = "demohollidaycodes@gmail.com",
                FirstName = "Demo",
                LastName = "Admin",
                EmailConfirmed = true
            };
            try
            {
                var user = await userManager.FindByEmailAsync(demoAdminUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(demoAdminUser, demoPassword);
                    await userManager.AddToRoleAsync(demoAdminUser, Roles.Admin.ToString());
                    await userManager.AddToRoleAsync(demoAdminUser, Roles.Demo.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("******** ERROR **********");
                Debug.WriteLine("Error Seeding Demo Admin User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("*************************");
                throw;
            }
            #endregion
            #region demoprojectmanageruser
            //Seed Project Manager User
            var demoPmUser = new BTUser
            {
                UserName = "demojaxholliday@gmail.com",
                Email = "demojaxholliday@gmail.com",
                FirstName = "Demo",
                LastName = "Pro. Manager",
                EmailConfirmed = true
            };
            try
            {
                var user = await userManager.FindByEmailAsync(demoPmUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(demoPmUser, demoPassword);
                    await userManager.AddToRoleAsync(demoPmUser, Roles.ProjectManager.ToString());
                    await userManager.AddToRoleAsync(demoPmUser, Roles.Demo.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("******** ERROR **********");
                Debug.WriteLine("Error Seeding Demo Admin User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("*************************");
                throw;
            }
            #endregion
            #region demodeveloper
            //Seed Developer User
            var demoDevUser = new BTUser
            {
                UserName = "demojeffholliday11@gmail.com",
                Email = "demojeffholliday11@gmail.com",
                FirstName = "Demo",
                LastName = "Developer",
                EmailConfirmed = true
            };
            try
            {
                var user = await userManager.FindByEmailAsync(demoDevUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(demoDevUser, demoPassword);
                    await userManager.AddToRoleAsync(demoDevUser, Roles.Developer.ToString());
                    await userManager.AddToRoleAsync(demoDevUser, Roles.Demo.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("******** ERROR **********");
                Debug.WriteLine("Error Seeding Default Admin User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("*************************");
                throw;
            }
            #endregion
            #region demosubmitter
            //Seed submitter User
            var demoSubUser = new BTUser
            {
                UserName = "demojaxhollidayau@gmail.com",
                Email = "demojaxhollidayau@gmail.com",
                FirstName = "Demo",
                LastName = "Submitter",
                EmailConfirmed = true
            };
            try
            {
                var user = await userManager.FindByEmailAsync(demoSubUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(demoSubUser, demoPassword);
                    await userManager.AddToRoleAsync(demoSubUser, Roles.Submitter.ToString());
                    await userManager.AddToRoleAsync(demoSubUser, Roles.Demo.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("******** ERROR **********");
                Debug.WriteLine("Error Seeding demo Submitter User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("*************************");
                throw;
            }
            #endregion
            #region demonewuser
            //Seed submitter User
            var demoNewUser = new BTUser
            {
                UserName = "demoNewUserhollidayau@gmail.com",
                Email = "demoNewUserjaxhollidayau@gmail.com",
                FirstName = "Aussie",
                LastName = "Outback",
                EmailConfirmed = true
            };
            try
            {
                var user = await userManager.FindByEmailAsync(demoNewUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(demoNewUser, demoPassword);
                    await userManager.AddToRoleAsync(demoNewUser, Roles.NewUser.ToString());
                    await userManager.AddToRoleAsync(demoNewUser, Roles.Demo.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("******** ERROR **********");
                Debug.WriteLine("Error Seeding demo Submitter User.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("*************************");
                throw;
            }
            #endregion

        }

        #region SeedTicketTypes
        public static async Task SeedDefaultTicketTypeAsync(ApplicationDbContext context)
        {
            try
            {
                if (!context.TicketTypes.Any(t => t.Name == "Runtime"))
                {
                    await context.TicketTypes.AddAsync(new TicketType { Name = "Runtime" });
                }
                if (!context.TicketTypes.Any(t => t.Name == "UI"))
                {
                    await context.TicketTypes.AddAsync(new TicketType { Name = "UI" });
                }
                if (!context.TicketTypes.Any(t => t.Name == "Backend"))
                {
                    await context.TicketTypes.AddAsync(new TicketType { Name = "Backend" });
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Ticket Types.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }

        }
        #endregion
        #region SeedTicketStatuses
        public static async Task SeedDefaultTicketStatusAsync(ApplicationDbContext context)
        {
            try
            {
                if (!context.TicketStatuses.Any(t => t.Name == "New"))
                {
                    await context.TicketStatuses.AddAsync(new TicketStatus { Name = "New" });
                }
                if (!context.TicketStatuses.Any(t => t.Name == "Closed"))
                {
                    await context.TicketStatuses.AddAsync(new TicketStatus { Name = "Closed" });
                }
                if (!context.TicketStatuses.Any(t => t.Name == "In-Progress"))
                {
                    await context.TicketStatuses.AddAsync(new TicketStatus { Name = "In-Progress" });
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Ticket Statuses.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }
        #endregion
        #region SeedTicketPriorities
        public static async Task SeedDefaultTicketPriorityAsync(ApplicationDbContext context)
        {
            try
            {
                if (!context.TicketPriorities.Any(t => t.Name == "Low"))
                {
                    await context.TicketPriorities.AddAsync(new TicketPriority { Name = "Low" });
                }
                if (!context.TicketPriorities.Any(t => t.Name == "High"))
                {
                    await context.TicketPriorities.AddAsync(new TicketPriority { Name = "High" });
                }
                if (!context.TicketPriorities.Any(t => t.Name == "Urgent"))
                {
                    await context.TicketPriorities.AddAsync(new TicketPriority { Name = "Urgent" });
                }
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("*************  ERROR  *************");
                Debug.WriteLine("Error Seeding Ticket Priorities.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("***********************************");
                throw;
            }
        }
        #endregion
        public static async Task SeedProjectsAsync(ApplicationDbContext context)
        {
            Project seedProject1 = new Project
            {
                Name = "Blog Project"
            };
            try
            {
                var project = await context.Projects.FirstOrDefaultAsync(p => p.Name == "Blog Project");
                if (project == null)
                {
                    await context.Projects.AddAsync(seedProject1);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("************* ERROR *************");
                Debug.WriteLine("Error Seeding Default Project 1.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("*********************************");
            };

            Project seedProject2 = new Project
            {
                Name = "Bug Tracker Project"
            };
            try
            {
                var project = await context.Projects.FirstOrDefaultAsync(p => p.Name == "Bug Tracker Project");
                if (project == null)
                {
                    await context.Projects.AddAsync(seedProject2);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("************* ERROR *************");
                Debug.WriteLine("Error Seeding Default Project 2.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("*********************************");
            };

            Project seedProject3 = new Project
            {
                Name = "Financial Portal Project"
            };
            try
            {
                var project = await context.Projects.FirstOrDefaultAsync(p => p.Name == "Financial Portal Project");
                if (project == null)
                {
                    await context.Projects.AddAsync(seedProject3);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("************* ERROR *************");
                Debug.WriteLine("Error Seeding Default Project 3.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("*********************************");
            };

        }
        //public static async Task SeedProjectUsersAsync(ApplicationDbContext context, UserManager<BTUser> userManager)
        //{
        //    string adminId = (await userManager.FindByEmailAsync("hollidaycodes@gmail.com")).Id;
        //    string projectManagerId = (await userManager.FindByEmailAsync("jaxholliday@gmail.com")).Id;
        //    string developerId = (await userManager.FindByEmailAsync("jeffholliday11@gmail.com")).Id;
        //    string submitterId = (await userManager.FindByEmailAsync("jaxhollidayau@gmail.com")).Id;
        //    int project1Id = (await context.Projects.FirstOrDefaultAsync(p => p.Name == "Blog Project")).Id;
        //    int project2Id = (await context.Projects.FirstOrDefaultAsync(p => p.Name == "Bug Tracker Project")).Id;
        //    int project3Id = (await context.Projects.FirstOrDefaultAsync(p => p.Name == "Financial Portal Project")).Id;
        //    ProjectUser projectUser = new ProjectUser
        //    {
        //        UserId = adminId,
        //        ProjectId = project1Id
        //    };
        //    try
        //    {
        //        var record = await context.ProjectUsers.FirstOrDefaultAsync(r => r.UserId == adminId && r.ProjectId == project1Id);
        //        if (record == null)
        //        {
        //            await context.ProjectUsers.AddAsync(projectUser);
        //            await context.SaveChangesAsync();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("************* ERROR *************");
        //        Debug.WriteLine("Error Seeding Admin Project 1.");
        //        Debug.WriteLine(ex.Message);
        //        Debug.WriteLine("*********************************");
        //    };
        //    projectUser = new ProjectUser
        //    {
        //        UserId = adminId,
        //        ProjectId = project2Id
        //    };
        //    try
        //    {
        //        var record = await context.ProjectUsers.FirstOrDefaultAsync(r => r.UserId == adminId && r.ProjectId == project2Id);
        //        if (record == null)
        //        {
        //            await context.ProjectUsers.AddAsync(projectUser);
        //            await context.SaveChangesAsync();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("************* ERROR *************");
        //        Debug.WriteLine("Error Seeding Admin Project 2.");
        //        Debug.WriteLine(ex.Message);
        //        Debug.WriteLine("*********************************");
        //    };
        //    projectUser = new ProjectUser
        //    {
        //        UserId = adminId,
        //        ProjectId = project3Id
        //    };
        //    try
        //    {
        //        var record = await context.ProjectUsers.FirstOrDefaultAsync(r => r.UserId == adminId && r.ProjectId == project3Id);
        //        if (record == null)
        //        {
        //            await context.ProjectUsers.AddAsync(projectUser);
        //            await context.SaveChangesAsync();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("************* ERROR *************");
        //        Debug.WriteLine("Error Seeding Admin Project 3.");
        //        Debug.WriteLine(ex.Message);
        //        Debug.WriteLine("*********************************");
        //    };
        //    projectUser = new ProjectUser
        //    {
        //        UserId = projectManagerId,
        //        ProjectId = project1Id
        //    };
        //    try
        //    {
        //        var record = await context.ProjectUsers.FirstOrDefaultAsync(r => r.UserId == projectManagerId && r.ProjectId == project1Id);
        //        if (record == null)
        //        {
        //            await context.ProjectUsers.AddAsync(projectUser);
        //            await context.SaveChangesAsync();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("************* ERROR *************");
        //        Debug.WriteLine("Error Seeding PM Project 1.");
        //        Debug.WriteLine(ex.Message);
        //        Debug.WriteLine("*********************************");
        //    };
        //    projectUser = new ProjectUser
        //    {
        //        UserId = projectManagerId,
        //        ProjectId = project2Id
        //    };
        //    try
        //    {
        //        var record = await context.ProjectUsers.FirstOrDefaultAsync(r => r.UserId == projectManagerId && r.ProjectId == project2Id);
        //        if (record == null)
        //        {
        //            await context.ProjectUsers.AddAsync(projectUser);
        //            await context.SaveChangesAsync();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("************* ERROR *************");
        //        Debug.WriteLine("Error Seeding PM Project 2.");
        //        Debug.WriteLine(ex.Message);
        //        Debug.WriteLine("*********************************");
        //    };
        //    projectUser = new ProjectUser
        //    {
        //        UserId = projectManagerId,
        //        ProjectId = project3Id
        //    };
        //    try
        //    {
        //        var record = await context.ProjectUsers.FirstOrDefaultAsync(r => r.UserId == projectManagerId && r.ProjectId == project3Id);
        //        if (record == null)
        //        {
        //            await context.ProjectUsers.AddAsync(projectUser);
        //            await context.SaveChangesAsync();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("************* ERROR *************");
        //        Debug.WriteLine("Error Seeding PM Project 3.");
        //        Debug.WriteLine(ex.Message);
        //        Debug.WriteLine("*********************************");
        //    };
        //}
        public static async Task SeedTicketsAsync(ApplicationDbContext context, UserManager<BTUser> userManager)
        {
            string developerId = (await userManager.FindByEmailAsync("jeffholliday11@gmail.com")).Id;
            string submitterId = (await userManager.FindByEmailAsync("jaxhollidayau@gmail.com")).Id;
            int project1Id = (await context.Projects.FirstOrDefaultAsync(p => p.Name == "Blog Project")).Id;
            int project2Id = (await context.Projects.FirstOrDefaultAsync(p => p.Name == "Bug Tracker Project")).Id;
            int project3Id = (await context.Projects.FirstOrDefaultAsync(p => p.Name == "Financial Portal Project")).Id;
            int statusId = (await context.TicketStatuses.FirstOrDefaultAsync(ts => ts.Name == "New")).Id;
            int typeId = (await context.TicketTypes.FirstOrDefaultAsync(tt => tt.Name == "UI")).Id;
            int priorityId = (await context.TicketPriorities.FirstOrDefaultAsync(tp => tp.Name == "Low")).Id;

            Ticket ticket = new Ticket
            {
                Title = "Need more blog posts",
                Description = "It's not a real blog when you only have a single post. Our users have requested you present more content. Without the content the Google crawlers will never up our organic ranking",
                Created = DateTimeOffset.Now.AddDays(-7),
                Updated = DateTimeOffset.Now.AddHours(-30),
                ProjectId = project1Id,
                TicketPriorityId = priorityId,
                TicketTypeId = typeId,
                TicketStatusId = statusId,
                DeveloperUserId = developerId,
                OwnerUserId = submitterId
            };
            try
            {
                var newTicket = await context.Tickets.FirstOrDefaultAsync(t => t.Title == "Need more blog posts");
                if (newTicket == null)
                {
                    await context.Tickets.AddAsync(ticket);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("************* ERROR *************");
                Debug.WriteLine("Error Seeding Ticket 1.");
                Debug.WriteLine(ex.Message);
                Debug.WriteLine("*********************************");
            };

        }

    }
}
