using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DragonTracker.Models
{
    public class TicketComment
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        
        public DateTimeOffset Created { get; set; }

        public int TicketId { get; set; }

        [Display(Name = "User")]
        public string UserId { get; set; }
        public virtual Ticket Ticket { get; set; }
        public virtual BTUser User { get; set; }
    }
}
