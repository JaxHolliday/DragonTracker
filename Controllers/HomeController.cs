using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DragonTracker.Models;
using Microsoft.AspNetCore.Authorization;
using DragonTracker.Models.ViewModels;
using DragonTracker.Data;
using DragonTracker.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DragonTracker.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IBTProjectService _projectService;
        private readonly UserManager<BTUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IBTProjectService projectService, UserManager<BTUser> userManager)
        {
            _logger = logger;
            _context = context;
            _projectService = projectService;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var dashboardVm = new DashboardViewModel();
            var userId = _userManager.GetUserId(User);
            dashboardVm.Projects = await _projectService.ListUserProjectsAsync(userId);



            //Only if Project Manager
            dashboardVm.Tickets = (await _context.Users.FirstOrDefaultAsync(u => u.Id == userId)).Projects.SelectMany(p => p.Tickets).ToList();
            return View(dashboardVm);
        }

    }
}
