// Infrastructure.Bislerium.Hubs.NotificationHubs
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Bislerium.Hubs
{
    public class NotificationHubs : Hub<INotificationClient>
    {
        public async Task SendNotificationToAll(string message)
        {
            await Clients.All.ReceiveNotification(message);
        }
    }

    public interface INotificationClient
    {
        Task ReceiveNotification(string message);
    }
}
