using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ControlGastos.Models;

namespace ControlGastos.ViewModels
{
    public class BorrarNotificationViewModel
    {
        private Notification notification;

        public BorrarNotificationViewModel(Notification notification)
        {
            this.notification = notification;
            var notificationViewModel = NotificationViewModel.GetInstance();
#pragma warning disable CS4014 // Ya que no se esperaba esta llamada, la ejecución del método actual continúa antes de que se complete la llamada
            notificationViewModel.Delete(this.notification);
#pragma warning restore CS4014 // Ya que no se esperaba esta llamada, la ejecución del método actual continúa antes de que se complete la llamada
        }
    }
}
