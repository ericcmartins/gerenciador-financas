using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace gerenciador.financas.Infra.Vendors
{
    public class NotificationPool
    {
        private readonly List<Notification> _notifications;
        public IReadOnlyCollection<Notification> Notifications => _notifications.AsReadOnly();
        public bool HasNotications => Notifications.Count > 0;

        public NotificationPool()
        {
            _notifications = [];
        }
        public void AddNotification(int code, string mensagem)
        {
            _notifications.Add(new Notification(code, mensagem));
        }
        public void AddNotification(Notification notification)
        {
            _notifications.Add(notification);
        }
    }
}
