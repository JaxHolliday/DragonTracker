using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DragonTracker.Models.ViewModels
{
    public class ManageUserRolesViewModel
    {
        public BTUser User { get; set; }

        public MultiSelectList Roles { get; set; }

        public string[] SelectedRoles { get; set; }


    }
}
