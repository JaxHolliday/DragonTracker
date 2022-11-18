using DragonTracker.Data;
using DragonTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DragonTracker.Services
{
    public class BTProjectService : IBTProjectService
    {
        private readonly ApplicationDbContext _context;

        public BTProjectService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsUserOnProjectAsync(string userId, int projectId)
        {
            Project project = await _context.Projects.FirstOrDefaultAsync(u => u.Id == projectId);
            bool result = project.Users.Any(u => u.Id == userId);
            return result;
        }

        public async Task<List<Project>> ListUserProjectsAsync(string userId)
        {
            try
            {
                List<Project> userProjects = (await _context.Users
                        .Include(u => u.Projects)
                            .ThenInclude(p => p.Tickets)
                        .Include(u => u.Projects)
                            .ThenInclude(t => t.Tickets)
                                .ThenInclude(t => t.DeveloperUser)
                        .Include(u => u.Projects)
                            .ThenInclude(t => t.Tickets)
                                .ThenInclude(t => t.OwnerUser)
                        .Include(u => u.Projects)
                            .ThenInclude(t => t.Tickets)
                                .ThenInclude(t => t.TicketPriority)
                        .Include(u => u.Projects)
                            .ThenInclude(t => t.Tickets)
                                .ThenInclude(t => t.TicketStatus)
                        .Include(u => u.Projects)
                            .ThenInclude(t => t.Tickets)
                                .ThenInclude(t => t.TicketType)
                        .FirstOrDefaultAsync(u => u.Id == userId)).Projects.ToList();
                return userProjects;
            }catch(Exception ex)
            { 
                Debug.WriteLine("*** ERROR *** - Error Getting user projects list.  -->" + ex.Message);
                throw;
            }
        }

        public async Task AddUserToProjectAsync(string userId, int projectId)
        {
            try
            {
                BTUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user != null)
                {
                    Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
                    if (!await IsUserOnProjectAsync(userId, projectId))
                    {
                        try
                        {
                            project.Users.Add(user);
                            await _context.SaveChangesAsync();
                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR *** - Error Adding user to project.  --> {ex.Message}");
            }
        }


        public async Task RemoveUserFromProjectAsync(string userId, int projectId)
        {
            try
            {
                BTUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                Project project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
                if (await IsUserOnProjectAsync(userId, projectId))
                {
                    try
                    {
                        project.Users.Remove(user);
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"*** ERROR *** - Error Adding user to project.  --> {ex.Message}");
            }
        }

        public async Task<ICollection<BTUser>> UsersNotOnProject(int projectId)
        {
            return await _context.Users.Where(u => IsUserOnProjectAsync(u.Id, projectId).Result == false).ToListAsync();
        }

    }
}

