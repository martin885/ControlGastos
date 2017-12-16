using ControlGastos.Models;
using ControlGastos.Services;
using ControlGastos.Views;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ControlGastos.ViewModels
{
    public class EditarNotificationViewModel
    {
        #region Services
        NavigationService navigationService;
        DialogService dialogService;
        #endregion

        #region Propiedades y atributos
        private Notification notification;
        public int NotificationId { get; set; }
        public string Title { get; set; }
        public string Hora { get; set; }
        public string Minutos { get; set; }
        public TimeSpan TiempoRestanteEnvio { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Anio { get; set; }
        public string Mes { get; set; }
        public string Dia { get; set; }
        public string Message { get; set; }
        #endregion

        #region Command
        public ICommand GuardarCambioCommand
        {
            get
            {
                return new RelayCommand(GuardarCambio);
            }
        }

        private async void GuardarCambio()
        {
            try
            {

                if (Date < DateTime.Now && Time < DateTime.Now.TimeOfDay.Subtract(TimeSpan.FromMinutes(1)))
                {
                    await dialogService.ShowMessage("Error", "El horario de entrega seleccionado no puede ser anterior al horario actual");

                    return;
                }

                IFormatProvider culture = new CultureInfo("es-ES");


                if (string.IsNullOrEmpty(Message))
                {
                    await dialogService.ShowMessage("Error", "Debe contener un mensaje para enviar");
                    return;
                }
                notification.Fecha = Date;
                notification.Anio = Date.ToString("yyyy", culture);
                notification.Mes = Date.ToString("MMM", culture);
                notification.Dia = Date.ToString("dd", culture);
                notification.Horario = Time;
                if (int.Parse(Time.Hours.ToString(culture)) < 10)
                {
                    notification.Hora = string.Format("0{0}", Time.Hours.ToString(culture));
                }
                else
                {
                    notification.Hora = Time.Hours.ToString(culture);
                }
                if (int.Parse(Time.Minutes.ToString(culture)) < 10)
                {
                    notification.Minutos = string.Format("0{0}", Time.Minutes.ToString(culture));
                }
                else
                {
                    notification.Minutos = Time.Minutes.ToString(culture);
                }

                notification.Fecha = Date.Date;

                notification.Horario = Time;

                if (string.IsNullOrEmpty(Title) || string.IsNullOrWhiteSpace(Title))
                {

                    notification.Title = "Aviso";
                }
                else
                {

                    notification.Title = string.Format("{0}{1}",Title.Substring(0, 1).ToUpper(), Title.Substring(1)); ;
                }

                notification.Title = Title;
                notification.Message = Message;

                var notificationViewModel = NotificationViewModel.GetInstance();
                await notificationViewModel.Editar(notification);
                var editarNotificationView = EditarNotificationView.GetInstance();
                await editarNotificationView.Navigation.PopAsync();
            }
            catch
            {
                await dialogService.ShowMessage("Error", "El formato elegido es incorrecto");
                return;
            }
        }
        #endregion

        #region Constructor
        public EditarNotificationViewModel(Notification notification)
        {

            navigationService = new NavigationService();
            dialogService = new DialogService();

            this.notification = notification;
            //Carga los valores en la página de edición
            Dia = notification.Dia;
            Mes = notification.Mes;
            Anio = notification.Anio;
            if (string.IsNullOrEmpty(notification.Title))
            {
                Title = "Aviso";
            }
            else
            {
                Title = notification.Title;
            }

            Message = notification.Message;

            Date = notification.Fecha;

            Time = notification.Horario;
            #endregion
        }
    }
}