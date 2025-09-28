using ChatAppApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatAppApi.Repositories
{
    public class NotificationRepository
    {
        private readonly AppDbContext _context;

        public NotificationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Notification>> FindLatestByUserAsync(User user, int quantity)
        {
            List<Notification> notifications = await _context.Notification.Where(n => n.User == user)
                .OrderByDescending(n => n.CreatedAt)
                .Take(quantity)
                .ToListAsync();
            return notifications;
        }

        public async Task UpdateReadForUnreadNotificationsByUserAsync(User user)
        {
            List<Notification> unreadNotifications = await _context.Notification.Where(n => n.User == user && n.IsRead == false).ToListAsync();
            foreach(Notification notif in unreadNotifications) {
                notif.IsRead = true;
            }
            await _context.SaveChangesAsync();
        }
    }
}
