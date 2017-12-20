using ControlGastos.Services;
using ControlGastos.ViewModels;
using ControlGastos.Views;
using GalaSoft.MvvmLight.Command;
using Plugin.LocalNotifications;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
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

        public string HorarioString { get; set; }

        public TimeSpan TiempoRestanteEnvio { get; set; }

        public DateTime Fecha { get; set; }

        public TimeSpan Horario { get; set; }

        public string FechaString { get; set; }

        public string Message { get; set; }

        public string CantidadDeDias { get; set; }

        public bool TodosLosDias { get; set; }

        public bool TodosLosDiasActivado { get; set; }

        public bool IsVisible { get; set; }

        public bool Toggled { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<NotificacionDiaria> ListaNotificacionDiaria { get; set; }

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
            if (TodosLosDias.Equals(true))
            {
                foreach(var notifDiaria in ListaNotificacionDiaria)
                {
                    if (notifDiaria.Horario <= DateTime.Now.TimeOfDay.Add(TimeSpan.FromMinutes(5)) && notifDiaria.Equals(DateTime.Now))
                    {
                        await dialogService.ShowMessage("Error", "La fecha del mensaje está próxima a su envío, no puede ser editado. Intentarlo de nuevo dentro de unos minutos.");
                        return;
                    }
                }

            }
            else
            {
                if (this.Horario <= DateTime.Now.TimeOfDay.Add(TimeSpan.FromMinutes(5)) && Fecha.Equals(DateTime.Now))
                {
                    await dialogService.ShowMessage("Error", "La fecha del mensaje está próxima a su envío, no puede ser editado");
                    return;
                }
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

        public ICommand ToggledCommand
        {
            get
            {
                return new RelayCommand(ToggledActive);
            }
        }

        private async void ToggledActive()
        {
            if (!Toggled)
            {
                await dialogService.ShowMessage("Mensaje", "Los mensajes diarios están desactivados");


                foreach (var notificacionDiaria in ListaNotificacionDiaria)
                {
                    CrossLocalNotifications.Current.Cancel(notificacionDiaria.NotificacionDiariaId);
                }
               
            }
          else
            {


                var FechayTiempo = Fecha.Date + Horario;

                TiempoRestanteEnvio = FechayTiempo - DateTime.Now;

                var tiempoSchedule = TiempoRestanteEnvio.TotalMinutes;

                foreach (var notificacionDiaria in ListaNotificacionDiaria)
                {
                    var contador = 1;

                    CrossLocalNotifications.Current.Show(
                         notificacionDiaria.Title,
                         notificacionDiaria.Message,
                         notificacionDiaria.NotificacionDiariaId,
                         DateTime.Now.AddMinutes(tiempoSchedule).AddDays(contador));

                    contador++;
                }
            }
            }
        }
        #endregion
    }

