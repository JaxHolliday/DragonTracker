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
    public class TicketTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TicketTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TicketTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.TicketTypes.ToListAsync());
        }

        [Authorize(Roles = "Admin, ProjectManager")]
        // GET: TicketTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketType = await _context.TicketTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketType == null)
            {
                return NotFound();
            }

            return View(ticketType);
        }

        [Authorize(Roles = "Admin, ProjectManager")]
        // GET: TicketTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin, ProjectManager")]
        // POST: TicketTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] TicketType ticketType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ticketType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ticketType);
        }

        [Authorize(Roles = "Admin, ProjectManager")]
        // GET: TicketTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketType = await _context.TicketTypes.FindAsync(id);
            if (ticketType == null)
            {
                return NotFound();
            }
            return View(ticketType);
        }

        [Authorize(Roles = "Admin, ProjectManager")]
        // POST: TicketTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] TicketType ticketType)
        {
            if (id != ticketType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticketType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketTypeExists(ticketType.Id))
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
            return View(ticketType);
        }

        [Authorize(Roles = "Admin, ProjectManager")]
        // GET: TicketTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticketType = await _context.TicketTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticketType == null)
            {
                return NotFound();
            }

            return View(ticketType);
        }

        [Authorize(Roles = "Admin, ProjectManager")]
        // POST: TicketTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticketType = await _context.TicketTypes.FindAsync(id);
            _context.TicketTypes.Remove(ticketType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketTypeExists(int id)
        {
            return _context.TicketTypes.Any(e => e.Id == id);
        }
    }
}
