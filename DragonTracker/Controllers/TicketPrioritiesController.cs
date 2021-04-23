using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DragonTracker.Data;
using DragonTracker.Models;
using Microsoft.AspNetCore.Authorization;

namespace DragonTracker.Controllers
{
    [Authorize]
    public class TicketPrioritiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketPrioritiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TicketPriorities
        public async Task<IActionResult> Index()
        {
            return View(await _context.TicketPriorities.ToListAsync());
        }

        [Authorize(Roles = "Admin, ProjectManager")]
        // GET: TicketPriorities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketPriority = await _context.TicketPriorities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketPriority == null)
            {
                return NotFound();
            }

            return View(ticketPriority);
        }

        [Authorize(Roles = "Admin, ProjectManager")]
        // GET: TicketPriorities/Create
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin, ProjectManager")]
        // POST: TicketPriorities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] TicketPriority ticketPriority)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ticketPriority);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ticketPriority);
        }

        [Authorize(Roles = "Admin, ProjectManager")]
        // GET: TicketPriorities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketPriority = await _context.TicketPriorities.FindAsync(id);
            if (ticketPriority == null)
            {
                return NotFound();
            }
            return View(ticketPriority);
        }

        [Authorize(Roles = "Admin, ProjectManager")]
        // POST: TicketPriorities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] TicketPriority ticketPriority)
        {
            if (id != ticketPriority.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticketPriority);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketPriorityExists(ticketPriority.Id))
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
            return View(ticketPriority);
        }

        [Authorize(Roles = "Admin, ProjectManager")]
        // GET: TicketPriorities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketPriority = await _context.TicketPriorities
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketPriority == null)
            {
                return NotFound();
            }

            return View(ticketPriority);
        }
        [Authorize(Roles = "Admin, ProjectManager")]
        // POST: TicketPriorities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticketPriority = await _context.TicketPriorities.FindAsync(id);
            _context.TicketPriorities.Remove(ticketPriority);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketPriorityExists(int id)
        {
            return _context.TicketPriorities.Any(e => e.Id == id);
        }
    }
}
