using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Infra.Vendors.Notification
{
    public class NotificationPool : INotificationPool
    {
        private readonly List<Notification> _notifications = new();
        public IReadOnlyCollection<Notification> Notifications => _notifications.AsReadOnly();
        public void AddNotification(int code, string mensagem)
        {
            _notifications.Add(new Notification(code, mensagem));
        }
        public void AddNotification(Notification notification)
        {
            _notifications.Add(notification);
        }
        public bool HasNotifications()
        {
            return _notifications.Any();
        }
    }
}
