using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DragonTracker.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }

        [Display(Name = "Ticket")]
        public int TicketId { get; set; }
        public string Property { get; set; }

        [Display(Name = "Previous")]
        public string OldValue { get; set; }

        [Display(Name = "New")]
        public string NewValue { get; set; }
        public DateTimeOffset Created { get; set; }

        [Display(Name = "User")]
        public string UserId { get; set; }
        public virtual Ticket Ticket { get; set; }
        public virtual BTUser User { get; set; }
    }
}
