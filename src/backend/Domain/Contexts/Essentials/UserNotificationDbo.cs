using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WeeControl.Core.Domain.Exceptions;
using WeeControl.Core.SharedKernel;
using WeeControl.Core.SharedKernel.Contexts.Essentials;

namespace WeeControl.Core.Domain.Contexts.Essentials;

[Table(nameof(UserNotificationDbo), Schema = nameof(Essentials))]
public class UserNotificationDbo : HomeNotificationModel
{
    public static UserNotificationDbo Create(Guid userid, string subject, string details, string link)
    {
        if (userid == Guid.Empty)
            throw new DomainOutOfRangeException("User ID must not be empty GUID");
        
        var notification = new UserNotificationDbo()
        {
            UserId = userid,
            Subject = subject,
            Body = details,
            NotificationUrl = link
        };
        
        notification.ThrowExceptionIfEntityModelNotValid();
        return notification;
    }
    
    public Guid UserId { get; set; }
    public UserDbo User { get; set; }

    private UserNotificationDbo()
    {
    }
}

public class UserNotificationEntityTypeConfig : IEntityTypeConfiguration<UserNotificationDbo>
{
    public void Configure(EntityTypeBuilder<UserNotificationDbo> builder)
    {
        builder.HasKey(x => x.NotificationId);
        builder.Property(x => x.NotificationId).HasDefaultValue(Guid.NewGuid());

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId);
        
        builder.Property(x => x.PublishedTs).HasDefaultValue(DateTime.UtcNow);
    }
}