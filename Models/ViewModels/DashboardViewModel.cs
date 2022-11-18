using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DragonTracker.Models.ViewModels
{
    public class DashboardViewModel
    {
        public ICollection<Project> Projects { get; set; } = new List<Project>();

        [Display(Name = "Members")]
        public ICollection<BTUser> ProjectUsers { get; set; } = new List<BTUser>();

        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

        public ICollection<TicketType> TicketTypes { get; set; } = new List<TicketType>();


        public int UrgentTicketCount { get; set; }

        public int UnassigenedTicketCount { get; set; }
        public string HighTicketCount { get; set; }
        public string LowTicketCount { get; set; }

    }
}
