using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DragonTracker.Models.ViewModels
{
    public class ProjectUsersViewModel
    {
        public Project Project { get; set; }
        
        public MultiSelectList Users { get; set; }  //populates list box

        public string[] SelectedUsers { get; set; }   // recieves selected Users
    }
}
