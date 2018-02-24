using ControlGastos.Models;
using ControlGastos.Services;
using ControlGastos.Views;
using GalaSoft.MvvmLight.Command;
using Plugin.LocalNotifications;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ControlGastos.ViewModels
{
   public class NotificationViewModel:INotifyPropertyChanged
    {
        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        
        #region Services
        DataService dataService;
        DialogService dialogService;
        #endregion

        #region Propiedades y Atributos
        IFormatProvider culture;

        public DateTime MinimuDate { get; set; }

        Notification Notificacion;

        string _mensajeNotification;
        public string MensajeNotification
        {
            get
            {
                return _mensajeNotification;
            }
            set
            {
                if (_mensajeNotification != value)
                {
                    _mensajeNotification = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MensajeNotification)));
                }
            }
        }
        ObservableCollection<Notification> _collectionNotification;
        public ObservableCollection<Notification> CollectionNotification
        {
            get
            {
                return _collectionNotification;
            }
            set
            {
                if (_collectionNotification != value)
                {
                    _collectionNotification = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CollectionNotification)));
                }
            }
        }

        string _tituloNotification;
        public string TituloNotification
        {
            get
            {
                return _tituloNotification;
            }
            set
            {
                if (_tituloNotification != value)
                {
                    _tituloNotification = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TituloNotification)));
                }
            }
        }

        public TimeSpan _time;
        public TimeSpan Time
        {
            get
            {
                return _time;
            }
            set
            {
                if (_time != value)
                {
                    _time = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Time)));
                }
            }
        }

        public DateTime _date;
        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                if (_date != value)
                {
                    _date = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Date)));
                }
            }
        }

        List<Notification> _listaNotifications;
        public List<Notification> ListaNotifications
    {
            get
            {
                return _listaNotifications;
            }
            set
            {
                if (_listaNotifications != value)
                {
                    _listaNotifications = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ListaNotifications)));
                }
            }
        }

               bool _isRefreshing;
        public bool IsRefreshing
    {
            get
            {
                return _isRefreshing;
            }
            set
            {
                if (_isRefreshing != value)
                {
                    _isRefreshing = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRefreshing)));
                }
            }
        }
        #endregion

        #region Commands

        public ICommand NotificationCommand
        {
            get
            {
                return new RelayCommand(Notification);
            }
        }

        private async void Notification()
        {

            //Crea el objeto Ingreso, lo agrego a la lista del mes, y después se hace la sumatoria de la lista

            if (string.IsNullOrEmpty(MensajeNotification) || string.IsNullOrWhiteSpace(MensajeNotification))
            {           
                await dialogService.ShowMessage("Error", "Debe contener un mensaje para enviar");
                return;
            }
            if (Date < DateTime.Now && Time<DateTime.Now.TimeOfDay.Subtract(TimeSpan.FromMinutes(1)))
            {
                await dialogService.ShowMessage("Error", "El horario de entrega seleccionado no puede ser anterior al horario actual");

                return;
            }

            Notificacion = new Notification();
            Notificacion.Anio = Date.ToString("yyyy", culture);
            Notificacion.Mes = Date.ToString("MMM", culture);
            Notificacion.Dia = Date.ToString("dd", culture);
            if (int.Parse(Time.Hours.ToString(culture)) < 10)
            {
                Notificacion.Hora =string.Format("0{0}", Time.Hours.ToString(culture));
            }
            else
            {
                Notificacion.Hora = Time.Hours.ToString(culture);
            }
            if(int.Parse(Time.Minutes.ToString(culture)) < 10)
            {
                Notificacion.Minutos = string.Format("0{0}", Time.Minutes.ToString(culture));
            }
            else
            {
                Notificacion.Minutos = Time.Minutes.ToString(culture);
            }

            var FechayTiempo = Date.Date + Time;

            Notificacion.Fecha = Date.Date;

            Notificacion.Horario = Time;

            Notificacion.TiempoRestanteEnvio = FechayTiempo - DateTime.Now  ;

            var tiempoSchedule = Notificacion.TiempoRestanteEnvio.TotalMinutes;

            if (string.IsNullOrEmpty(TituloNotification) || string.IsNullOrWhiteSpace(TituloNotification))
            {

                Notificacion.Title = "Aviso";
            }
            else 
            {

                Notificacion.Title = string.Format("{0}{1}", TituloNotification.Substring(0, 1).ToUpper(), TituloNotification.Substring(1)); ;
            }
            Notificacion.Message = string.Format("{0}{1}",MensajeNotification.Substring(0,1).ToUpper(),MensajeNotification.Substring(1));
            ListaNotifications.Add(Notificacion);
            TituloNotification = null;
            MensajeNotification = null;
            Date = DateTime.Now;
            Time = DateTime.Now.TimeOfDay;
            dataService.Save(ListaNotifications, true);
            CollectionNotification = new ObservableCollection<Notification>(
                ListaNotifications.OrderByDescending(
                    x=>x.TiempoRestanteEnvio.TotalMinutes));
            try
            {
               CrossLocalNotifications.Current.Show(
                    Notificacion.Title,
                    Notificacion.Message,
                    Notificacion.NotificationId,
                    DateTime.Now.AddMinutes(tiempoSchedule));

            }
            catch(Exception e)
            {
                await dialogService.ShowMessage("Error", e.Message);            
            }
        }

        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(Refresh);
            }
        }

        private void Refresh()
        {
            IsRefreshing = true;
            ListaNotifications = dataService.Get<Notification>(true);
            foreach (var notification in ListaNotifications)
            {
                var fechaTiempo = notification.Fecha.Date + notification.Horario;

                if (fechaTiempo <= DateTime.Now)
                {
                    dataService.Delete(notification);

                }
            }
            ListaNotifications = dataService.Get<Notification>(true);
            CollectionNotification = new ObservableCollection<Notification>(ListaNotifications.OrderByDescending(x => x.TiempoRestanteEnvio.TotalMinutes));
            IsRefreshing = false;
        }

        #endregion

        #region Constructor
        public NotificationViewModel()
        {
            dataService = new DataService();
            dialogService = new DialogService();
            culture = new CultureInfo("es-ES");
            instance = this;

            Cargas();

        }
        #endregion

        #region Singleton

        static NotificationViewModel instance;

        public static NotificationViewModel GetInstance()
        {
            if (instance == null)
            {
                return new NotificationViewModel();
            }
            return instance;
        }
        #endregion

        #region Métodos
        private void Cargas()
        {
            Date = DateTime.Now;
            MinimuDate = DateTime.Now;
            Time = DateTime.Now.TimeOfDay;
            ListaNotifications = new List<Notification>();
            if (dataService.CheckTableIsEmpty<Notification>())
            {
                ListaNotifications = dataService.Get<Notification>(true);
               foreach(var notification in ListaNotifications)
                {
                    var fechaTiempo = notification.Fecha.Date + notification.Horario;

                    if (fechaTiempo <= DateTime.Now)
                    {
                        dataService.Delete(notification);
                        
                    }
                }
                ListaNotifications = dataService.Get<Notification>(true);
                CollectionNotification = new ObservableCollection<Notification>(ListaNotifications.OrderByDescending(x=>x.TiempoRestanteEnvio.TotalMinutes));
            }

        }

        public async Task Editar(Notification notification)
        {
            var notificacionAntigua = ListaNotifications.Find(x => x.NotificationId == notification.NotificationId);
            notificacionAntigua = notification;
            dataService.Update(notificacionAntigua, true);

            var FechayTiempo = notification.Fecha.Date + notification.Horario;

            notification.TiempoRestanteEnvio = FechayTiempo - DateTime.Now;

            var tiempoSchedule = notification.TiempoRestanteEnvio.TotalMinutes;

            try
            {
                CrossLocalNotifications.Current.Cancel(notification.NotificationId);
                CrossLocalNotifications.Current.Show(
                     notification.Title,
                     notification.Message,
                     notification.NotificationId,
                     DateTime.Now.AddMinutes(tiempoSchedule));

            }
            catch (Exception e)
            {
                await dialogService.ShowMessage("Error", e.Message);
            }

            CollectionNotification = new ObservableCollection<Notification>(ListaNotifications.OrderByDescending(x => x.TiempoRestanteEnvio.TotalMinutes));
        }

        public async Task Delete(Notification notification)
        {
            var confirmacion = await dialogService.ShowMessageConfirmacion("Mensaje", "¿Desea borrar este elemento?");

            if (confirmacion)
            {

                var notificacionAntigua = ListaNotifications.Find(x => x.NotificationId == notification.NotificationId);
                CrossLocalNotifications.Current.Cancel(notification.NotificationId);
                dataService.Delete(notificacionAntigua);
                ListaNotifications.Remove(notification);

                CollectionNotification = new ObservableCollection<Notification>(ListaNotifications.OrderByDescending(x => x.TiempoRestanteEnvio.TotalMinutes));
            }
            else
            {
                return;
            }
        }
        #endregion

    }
}
