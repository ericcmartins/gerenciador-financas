using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace gerenciador.financas.Infra.Vendors.Notification
{
    public interface INotificationPool
    {
        void AddNotification(int code, string mensagem);
        void AddNotification(Notification notification);
        bool HasNotifications();
    }
}

