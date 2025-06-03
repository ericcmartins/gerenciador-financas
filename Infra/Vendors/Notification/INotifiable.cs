using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace gerenciador.financas.Infra.Vendors
{
    public interface INotifiable
    {
        bool HasNotifications { get; }
        IReadOnlyCollection<Notification> Notifications { get; }   
    }
}

