using DragonTracker.Data;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DragonTracker.Services
{
    public class BTAccessService : IBTAccessService
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTProjectService _projectService;

        public BTAccessService(ApplicationDbContext context,
                                IBTProjectService projectService)
        {
            _context = context;
            _projectService = projectService;
        }

        public async Task<bool> CanInteractProject(string userId, int projectId, string roleName)
        {
            switch (roleName)
            {
                case "Admin":
                    return true;
                case "ProjectManager":
                    if (await _projectService.IsUserOnProjectAsync(userId,projectId))
                    {
                        return true;
                    }
                    return false;
                default:
                    return false;
            }
        }

        public async Task<bool> CanInteractTicket(string userId, int ticketId, string roleName)
        {
            bool result = false;
            switch (roleName)
            {
                case "Admin":
                    result = true;
                    break;
                case "ProjectManager":
                    var projectId = (await _context.Tickets.FindAsync(ticketId)).ProjectId;
                    if (await _context.Projects.AnyAsync(p => p.Id == projectId))
                    {
                        result = true;                        
                    }                    
                    break;
                case "Developer":
                    if (await _context.Tickets.Where(t => t.DeveloperUserId == userId && t.Id == ticketId).AnyAsync())
                    {
                        result = true;
                    }
                    break;
                case "Submitter":
                    if (await _context.Tickets.Where(t => t.OwnerUserId == userId && t.Id == ticketId).AnyAsync())
                    {
                        result = true;
                    }
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}
