using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DragonTracker.Data;
using DragonTracker.Models;
using DragonTracker.Services;
using DragonTracker.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace DragonTracker.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IBTProjectService _btprojectService;

        public ProjectsController(ApplicationDbContext context, IBTProjectService projectService)
        {
            _context = context;
            _btprojectService = projectService;
        }

        [Authorize(Roles = "Admin, ProjectManager")]
        // GET: Projects
        public async Task<IActionResult> Index()
        {
            return View(await _context.Projects.ToListAsync());
        }
        [Authorize(Roles = "Admin, ProjectManager")]
        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Project project = await _context.Projects
                .Include(p => p.Users)
                .Include(p => p.Tickets)                
                .ThenInclude(t => t.TicketType)
                .Include(p => p.Tickets)
                .ThenInclude(t => t.TicketStatus)
                .Include(p => p.Tickets)
                .ThenInclude(t => t.TicketPriority)
                .FirstOrDefaultAsync(m => m.Id == id);  //go into DB, then into Projects Table, find 1st PRO with this ID, only info on that project

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        [Authorize(Roles = "Admin, ProjectManager")]
        // GET: Projects/Create
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin, ProjectManager")]
        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to         
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ImagePath,ImageData")] Project project)
        {
            if (ModelState.IsValid)
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
                //ProjectUser record = new ProjectUser { UserId = _userManager.GetUserId(User), ProjectId = }
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }
        [Authorize(Roles = "Admin, ProjectManager, Developer")]
        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }
        [Authorize(Roles = "Admin, ProjectManager")]
        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ImagePath,ImageData")] Project project)
        {
            if (!User.IsInRole("Demo"))
            {
                if (id != project.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(project);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ProjectExists(project.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(project);
            }
            else
            {
                TempData["DemoLockout"] = " Your changes have not been saved. This is a demo role. ";
                return RedirectToAction("Details", "Project", new { id = project.Id });
            }

        }


        [Authorize(Roles = "Admin, ProjectManager")]
        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }
        [Authorize(Roles = "Admin, ProjectManager")]
        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }

        [Authorize(Roles = "Admin, ProjectManager")]
        //Get For Assigned Users
        [HttpGet]
        public async Task<IActionResult> AssignUsers(int id)
        {
            var model = new ProjectUsersViewModel();
            var project = _context.Projects.Find(id);

            model.Project = project;
            List<BTUser> users = await _context.Users.ToListAsync();
            List<BTUser> members = project.Users.ToList();
            model.Users = new MultiSelectList(users, "Id", "FullName", members);
            return View(model);
        }
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignUsers(ProjectUsersViewModel model)
        {
            if (model.SelectedUsers != null)
            {
                List<string> memberIds = (await _context.Projects.FirstOrDefaultAsync(p => p.Id == model.Project.Id)).Users.Select(u=>u.Id).ToList();

                //memberIds.ForEach(i => _btprojectService.RemoveUserFromProject(i, model.Project.Id));
                //model.SelectedUsers.ToList().ForEach(i => _projectService.RemoveUserFromProject(i, model.Project.Id));

                foreach (string id in memberIds)
                {
                    await _btprojectService.RemoveUserFromProjectAsync(id, model.Project.Id);
                }

                foreach (string id in model.SelectedUsers)
                {
                    await _btprojectService.AddUserToProjectAsync(id, model.Project.Id);
                }
                return RedirectToAction("Index", "Projects");
            }
            else
            {
                //Send an error message back
            }
            return View(model);

        }
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpGet]
        public async Task<IActionResult> RemoveUsers(int id)
        {
            var model = new ProjectUsersViewModel();
            var project = _context.Projects.Find(id);

            model.Project = project;
            List<BTUser> users = await _context.Users.ToListAsync();
            List<BTUser> members = project.Users.ToList();
            model.Users = new MultiSelectList(users, "Id", "FullName", members);
            return View(model);
        }
        [Authorize(Roles = "Admin, ProjectManager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveUsers(ProjectUsersViewModel model)
        {
            if (model.SelectedUsers != null)
            {

                foreach (string id in model.SelectedUsers)
                {
                    await _btprojectService.RemoveUserFromProjectAsync(id, model.Project.Id);
                }
                return RedirectToAction("Index", "Projects");
            }
            else
            {
                //Send an error message back
            }
            return View(model);

        }

    }
}
