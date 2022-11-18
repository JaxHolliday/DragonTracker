using DragonTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DragonTracker.Services
{
    public interface IBTProjectService
    {
        public Task<bool> IsUserOnProjectAsync(string userId, int projectId);
        public Task<List<Project>> ListUserProjectsAsync(string userId);
        public Task AddUserToProjectAsync(string userId, int projectId);
        public Task RemoveUserFromProjectAsync(string userId, int projectId);
        public Task <ICollection<BTUser>> UsersNotOnProject(int projectId);
    }
}
