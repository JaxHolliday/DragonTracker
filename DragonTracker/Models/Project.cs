using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DragonTracker.Models
{
    public class Project
    {
        public Project()
        {
            Tickets = new HashSet<Ticket>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Project Name")]
        public string Name { get; set; }

        [Display(Name = "Project Image")]
        public string ImagePath { get; set; }
        public byte[] ImageData { get; set; }

        public virtual ICollection<BTUser> Users { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; }

    }
}
