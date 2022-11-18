using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DragonTracker.Data;
using DragonTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using DragonTracker.Services;
using Microsoft.AspNetCore.Http;

namespace DragonTracker.Controllers
{
    [Authorize]
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTHistoriesService _historyService;
        private readonly IBTProjectService _projectService;

        public TicketsController(ApplicationDbContext context, UserManager<BTUser> userManager, IBTHistoriesService historyService, IBTProjectService projectService)
        {
            _context = context;
            _userManager = userManager;
            _historyService = historyService;
            _projectService = projectService;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            //Creating a "new list" of type ticket
            var model = new List<Ticket>();
            var userId = _userManager.GetUserId(User);

            #region Possible Method for showing specific fields based on the user
            if (User.IsInRole("Admin"))
            {
                model = _context.Tickets
                    .Include(t => t.DeveloperUser)
                    .Include(t => t.OwnerUser)
                    .Include(t => t.Project)
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType).ToList();
            }
            else if (User.IsInRole("ProjectManager"))
            {

                List<Project> projs = await _projectService.ListUserProjectsAsync(userId);
                model = projs.SelectMany(t => t.Tickets).ToList();

            }
            else if (User.IsInRole("Developer"))
            {
                model = _context.Tickets
                    //Person trying to match 
                    .Where(t => t.DeveloperUserId == userId)

                    .Include(t => t.OwnerUser)
                    .Include(t => t.Project)
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType).ToList();
            }
            else if (User.IsInRole("Submitter"))
            {
                model = _context.Tickets
                    //Person trying to match 
                    .Where(t => t.OwnerUserId == userId)

                    .Include(t => t.OwnerUser)
                    .Include(t => t.Project)
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType).ToList();
            }
            else
            {
                return NotFound();
            }
            return View(model);

            #endregion

            //var applicationDbContext = _context.Tickets.Include(t => t.DeveloperUser).Include(t => t.OwnerUser).Include(t => t.Project).Include(t => t.TicketPriority).Include(t => t.TicketStatus).Include(t => t.TicketType);
            //return View(await applicationDbContext.ToListAsync());
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var ticket = await _context.Tickets
                .Include(t => t.DeveloperUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.Project)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .Include(t => t.Histories)
                .Include(t => t.Attachments)
                .Include(t => t.Comments).ThenInclude(tc => tc.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }


            ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "FullName", ticket.DeveloperUserId);
            ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "FullName", ticket.OwnerUserId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }

        [Authorize(Roles = "Admin, ProjectManager, Developer, Submitter")]
        // GET: Tickets/Create
        public IActionResult Create()
        {
            ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "FullName");
            ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "Name");

            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name");
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Name");
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name");

            if (User.IsInRole("Admin"))
            {
                ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
            }
            else
            {
                var userId = _userManager.GetUserId(User);
                var records = _context.Users.FirstOrDefault(u => u.Id == userId).Projects.ToList();
                ViewData["ProjectId"] = new SelectList(records, "Id", "Name");

            }

            if (!User.IsInRole("Admin") && !User.IsInRole("ProjectManager"))
            {
                var ticket = new Ticket
                {
                    TicketPriorityId = _context.TicketPriorities.Where(tp => tp.Name == "Low").FirstOrDefault().Id,
                    TicketStatusId = _context.TicketStatuses.Where(tp => tp.Name == "New").FirstOrDefault().Id
                };
                return View(ticket);
            }

            return View();
        }



        [Authorize(Roles = "Admin, ProjectManager, Developer, Submitter")]
        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,DeveloperUserId")] Ticket ticket, IFormFile attachment)
        {
            if (ModelState.IsValid)
            {
                ticket.Created = DateTimeOffset.Now;
                ticket.OwnerUserId = _userManager.GetUserId(User);

                TicketPriority priority = await _context.TicketPriorities.FirstOrDefaultAsync(t => t.Name == "Low");
                if (priority != null)
                {
                    ticket.TicketPriorityId = priority.Id;
                }

                TicketStatus status = await _context.TicketStatuses.FirstOrDefaultAsync(s => s.Name == "High");
                if (status != null)
                {
                    ticket.TicketStatusId = status.Id;
                }

                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Tickets", new { id = ticket.Id });
            }
            ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "FullName", ticket.DeveloperUserId);
            ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "FullName", ticket.OwnerUserId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }
        [Authorize(Roles = "Admin, ProjectManager, Developer")]
        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "FullName", ticket.DeveloperUserId);
            ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "FullName", ticket.OwnerUserId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
            ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
            ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
            ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name", ticket.TicketTypeId);
            return View(ticket);
        }
        [Authorize(Roles = "Admin, ProjectManager, Developer")]
        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Created,Updated,ProjectId,TicketTypeId,TicketPriorityId,TicketStatusId,OwnerUserId,DeveloperUserId")] Ticket ticket)
        {
            if (!User.IsInRole("Demo"))
            {
                ticket.Updated = DateTimeOffset.Now;


                if (id != ticket.Id)
                {
                    return NotFound();
                }
                // as no tracking --> Ticket to be read from and dont follow changes being made
                Ticket oldTicket = await _context.Tickets
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType)
                    .Include(t => t.Project)
                    .Include(t => t.DeveloperUser)
                    .AsNoTracking().FirstOrDefaultAsync(t => t.Id == ticket.Id);

                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(ticket);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!TicketExists(ticket.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    //Add History
                    string userId = _userManager.GetUserId(User);
                    Ticket newTicket = await _context.Tickets
                    .Include(t => t.TicketPriority)
                    .Include(t => t.TicketStatus)
                    .Include(t => t.TicketType)
                    .Include(t => t.Project)
                    .Include(t => t.DeveloperUser)
                    .AsNoTracking().FirstOrDefaultAsync(t => t.Id == ticket.Id);
                    await _historyService.AddHistory(oldTicket, newTicket, userId);


                    return RedirectToAction("Details", "Tickets", new { id = ticket.Id });
                }

                ViewData["DeveloperUserId"] = new SelectList(_context.Users, "Id", "FullName", ticket.DeveloperUserId);
                ViewData["OwnerUserId"] = new SelectList(_context.Users, "Id", "FullName", ticket.OwnerUserId);
                ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", ticket.ProjectId);
                ViewData["TicketPriorityId"] = new SelectList(_context.TicketPriorities, "Id", "Name", ticket.TicketPriorityId);
                ViewData["TicketStatusId"] = new SelectList(_context.TicketStatuses, "Id", "Name", ticket.TicketStatusId);
                ViewData["TicketTypeId"] = new SelectList(_context.TicketTypes, "Id", "Name", ticket.TicketTypeId);
                return RedirectToAction("Details", "Tickets", new { id = ticket.Id});
            }
            else
            {
                TempData["DemoLockout"] = " Your changes have not been saved. This is a demo role. ";
                return RedirectToAction("Details", "Project", new { id = ticket.ProjectId });
            }



        }
        [Authorize(Roles = "Admin, ProjectManager")]
        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.DeveloperUser)
                .Include(t => t.OwnerUser)
                .Include(t => t.Project)
                .Include(t => t.TicketPriority)
                .Include(t => t.TicketStatus)
                .Include(t => t.TicketType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }
        [Authorize(Roles = "Admin, ProjectManager")]
        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.Id == id);
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public async Task<FileResult> DownloadFile(int? id)
        {
            if(id == null)
            {
                return null;
            }
            TicketAttachment attachment = await _context.TicketAttachments.FirstOrDefaultAsync(t => t.Id == id);
            if(attachment == null)
            {
                return null;
            }
            return File(attachment.FileData, attachment.ContentType);
        }


    }
}
