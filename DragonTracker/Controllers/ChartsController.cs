using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonTracker.Data;
using DragonTracker.Models.ChartModels;
using Microsoft.AspNetCore.Mvc;

namespace DragonTracker.Controllers
{
    public class ChartsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public JsonResult GetBarChartData()
        {
            var data = new DemoData();

            var ticketPriorites = _context.TicketPriorities.ToList();

            foreach (var priority in ticketPriorites)
            {
                data.Labels.Add(priority.Name);
                data.Data.Add(_context.Tickets.Where(t => t.TicketPriorityId == priority.Id).Count());
            }
            return Json(data);
        }

        public JsonResult GetStatusChartData()
        {
            var data = new DemoData();

            var ticketStatuses = _context.TicketStatuses.ToList();

            foreach (var status in ticketStatuses)
            {
                data.Labels.Add(status.Name);
                data.Data.Add(_context.Tickets.Where(t => t.TicketStatusId == status.Id).Count());
            }
            return Json(data);
        }
    }
}
