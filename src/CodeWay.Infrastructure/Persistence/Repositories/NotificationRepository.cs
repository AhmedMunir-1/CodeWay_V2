namespace CodeWay.Infrastructure.Persistence.Repositories;

using CodeWay.Domain.Entities.Notifications;
using CodeWay.Domain.Interfaces.Repositories;

public class NotificationRepository : Repository<Notification>, INotificationRepository
{
    public NotificationRepository(ApplicationDbContext context) : base(context)
    {
    }
}
