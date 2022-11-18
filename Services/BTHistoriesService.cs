using DragonTracker.Data;
using DragonTracker.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace DragonTracker.Services
{
    public class BTHistoriesService : IBTHistoriesService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BTUser> _userManager;
        private readonly IEmailSender _emailSender;

        public BTHistoriesService(ApplicationDbContext context, UserManager<BTUser> userManager, IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task AddHistory(Ticket oldTicket, Ticket newTicket, string userId)
        {
            if (oldTicket.Title != newTicket.Title)
            {
                TicketHistory history = new TicketHistory
                {
                    TicketId = newTicket.Id,
                    Property = "Title",
                    OldValue = oldTicket.Title,
                    NewValue = newTicket.Title,
                    Created = DateTimeOffset.Now,
                    UserId = userId
                };
                await _context.TicketHistories.AddAsync(history);
            }
            if (oldTicket.Description != newTicket.Description)
            {
                TicketHistory history = new TicketHistory
                {
                    TicketId = newTicket.Id,
                    Property = "Description",
                    OldValue = oldTicket.Description,
                    NewValue = newTicket.Description,
                    Created = DateTimeOffset.Now,
                    UserId = userId
                };
                await _context.TicketHistories.AddAsync(history);
            }
            if (oldTicket.TicketTypeId != newTicket.TicketTypeId)
            {
                TicketHistory history = new TicketHistory
                {
                    TicketId = newTicket.Id,
                    Property = "Ticket Type",
                    OldValue = oldTicket.TicketType.Name,
                    NewValue = newTicket.TicketType.Name,
                    Created = DateTimeOffset.Now,
                    UserId = userId
                };
                await _context.TicketHistories.AddAsync(history);
            }
            if (oldTicket.TicketStatusId != newTicket.TicketStatusId)
            {
                TicketHistory history = new TicketHistory
                {
                    TicketId = newTicket.Id,
                    Property = "Ticket Status",
                    OldValue = oldTicket.TicketStatus.Name,
                    NewValue = newTicket.TicketStatus.Name,
                    Created = DateTimeOffset.Now,
                    UserId = userId
                };
                await _context.TicketHistories.AddAsync(history);
            }
            if (oldTicket.TicketPriority != newTicket.TicketPriority)
            {
                TicketHistory history = new TicketHistory
                {
                    TicketId = newTicket.Id,
                    Property = "Ticket Priority",
                    OldValue = oldTicket.TicketPriority.Name,
                    NewValue = newTicket.TicketPriority.Name,
                    Created = DateTimeOffset.Now,
                    UserId = userId
                };
                await _context.TicketHistories.AddAsync(history);
            }
            if (oldTicket.DeveloperUserId != newTicket.DeveloperUserId)
            {
                TicketHistory history = new TicketHistory
                {
                    TicketId = newTicket.Id,
                    Property = "Developer",
                    OldValue = oldTicket.DeveloperUser != null ? oldTicket.DeveloperUser.FullName : "Un-Assigned",
                    NewValue = newTicket.DeveloperUser != null ? newTicket.DeveloperUser.FullName : "Un-Assigned",
                    Created = DateTimeOffset.Now,
                    UserId = userId
                };
                await _context.TicketHistories.AddAsync(history);

                Notification notification = new Notification
                {
                    TicketId = newTicket.Id,
                    Description = "You Have a new Ticket.",
                    Created = DateTimeOffset.Now,
                    SenderId = userId,
                    RecipientId = newTicket.DeveloperUserId,
                };
                await _context.Notifications.AddAsync(notification);

                //send email
                string devEmail = newTicket.DeveloperUser.Email;
                string subject = "New Ticket Assignment";
                string message = $"You have a new ticket for project: {newTicket.Project.Name}";

                await _emailSender.SendEmailAsync(devEmail, subject, message);
            }

            await _context.SaveChangesAsync();
        }
    }
}
