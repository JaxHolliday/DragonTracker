using DragonTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DragonTracker.Services
{
    public interface IBTHistoriesService
    {
        public Task AddHistory(Ticket oldTicket, Ticket newTicket, string userId);        
    }
}
