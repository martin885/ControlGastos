using ControlGastos.Services;
using ControlGastos.ViewModels;
using ControlGastos.Views;
using GalaSoft.MvvmLight.Command;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ControlGastos.Models
{
    public class Notification
    {
        #region Services
        DialogService dialogService;
        #endregion

        #region Propiedades
        [PrimaryKey, AutoIncrement]
        public int NotificationId { get; set; }

        public string Title { get; set; }

        public string Hora { get; set; }

        public string Minutos { get; set; }

        public TimeSpan TiempoRestanteEnvio { get; set; }

        public DateTime Fecha { get; set; }

        public TimeSpan Horario { get; set; }

        public string Anio { get; set; }

        public string Mes { get; set; }

        public string Dia { get; set; }

        public string Message { get; set; }

        public override int GetHashCode()
        {
            return NotificationId;
        }
        #endregion

        #region Constructor
        public Notification()
        {
            dialogService = new DialogService();
        }
        #endregion

        #region Commands
        public ICommand EditCommand
        {
            get
            {
                return new RelayCommand(Edit);
            }
        }

        private async void Edit()
        {
            if (Horario <= DateTime.Now.TimeOfDay.Add(TimeSpan.FromMinutes(5))&& Fecha<=DateTime.Now)
            {
                await dialogService.ShowMessage("Error", "La fecha del mensaje está próxima a su envío, no puede ser editado");
                return;
            }

            //Instanciar ViewModel
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.EditarNotification = new EditarNotificationViewModel(this);

            //Ir a la página de edición
            var notificationView = NotificationView.GetInstance();
           await notificationView.Navigation.PushAsync(new EditarNotificationView());
        }

        public ICommand DeleteCommand
        {
            get
            {
                return new RelayCommand(Delete);
            }
        }

        private async void Delete()
        {
            if (this.Horario <= DateTime.Now.TimeOfDay.Add(TimeSpan.FromMinutes(5)))
            {
                await dialogService.ShowMessage("Error", "La fecha del mensaje está próxima a su envío, no puede ser borrado");
                return;
            }

            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.BorrarNotification = new BorrarNotificationViewModel(this);
        }
        #endregion
    }
}
