using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WeeControl.Domain.Interfaces;
using WeeControl.Domain.Models;

namespace WeeControl.Application.EssentialContext.Notifications;

public class PasswordResetNotification : INotification
{
    private readonly Guid userId;

    private readonly string newPassword;

    public PasswordResetNotification(Guid userid, string newPassword)
    {
        this.userId = userid;
        this.newPassword = newPassword;
    }
    
    public class PasswordResetHandler : INotificationHandler<PasswordResetNotification>
    {
        private readonly IEmailNotificationService notification;
        private readonly IEssentialDbContext context;

        public PasswordResetHandler(IEmailNotificationService notification, IEssentialDbContext context)
        {
            this.notification = notification;
            this.context = context;
        }
        
        public async Task Handle(PasswordResetNotification notif, CancellationToken cancellationToken)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.UserId == notif.userId, cancellationToken);
            if (user == null)
            {
                throw new NullReferenceException();
            }

            var message = GetMessage(user.Email, notif.newPassword);
            await notification.SendAsync(message);
            Console.WriteLine("New Password is: {0}", notif.newPassword);
        }
    }
    
    private static IMessageDto GetMessage(string to, string newPassword)
    {
        return new MessageDto()
        {
            To = to,
            Subject = "WeeControl - A New Password Has Been Created for You",
            Body = "Your new password is: " + newPassword
        };
    }
}