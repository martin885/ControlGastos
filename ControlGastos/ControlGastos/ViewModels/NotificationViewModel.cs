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
    public class NotificationViewModel : INotifyPropertyChanged
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
        public List<Notification> ListaNotifications { get; set; }
        public List<NotificacionDiaria> ListaNotificacionDiaria { get; set; }
        Notification Notificacion;
        NotificacionDiaria notificacionDiaria;
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

        bool _toggled;
        public bool Toggled
        {
            get
            {
                return _toggled;
            }
            set
            {
                if (_toggled != value)
                {
                    _toggled = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Toggled)));
                }
            }
        }
        bool _isVisible;
        public bool IsVisible
        {
            get
            {
                return _isVisible;
            }
            set
            {
                if (_isVisible != value)
                {
                    _isVisible = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsVisible)));
                }
            }
        }

        bool _isVisibleEntry;
        public bool IsVisibleEntry
        {
            get
            {
                return _isVisibleEntry;
            }
            set
            {
                if (_isVisibleEntry != value)
                {
                    _isVisibleEntry = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsVisibleEntry)));
                }
            }
        }

        string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Text)));
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
            try
            {
                //Crea el objeto Ingreso, lo agrego a la lista del mes, y después se hace la sumatoria de la lista
                if (IsVisibleEntry.Equals(false))
                {
                    Text = "1";
                }
                if (Text.Contains("-") || Text.Contains("."))
                {
                    await dialogService.ShowMessage("Error", "La cantidad de días debe contener un número entero y positivo");
                    return;
                }
                else if (string.IsNullOrEmpty(Text) && IsVisibleEntry.Equals(true))
                {
                    await dialogService.ShowMessage("Error", "La cantidad de días debe contener un número entero y positivo");
                    return;
                }
                if (int.Parse(Text) > 31)
                {
                    var confirmar = await dialogService.ShowMessageConfirmacion("Error", "La cantidad de días debe ser como máximo de 31, ¿desea que 31 sea el nuevo valor?");
                    if (confirmar)
                    {
                        Text = "31";
                    }
                    else
                    {
                        return;
                    }

                }

                if (string.IsNullOrEmpty(MensajeNotification) || string.IsNullOrWhiteSpace(MensajeNotification))
                {
                    await dialogService.ShowMessage("Error", "Debe contener un mensaje para enviar");
                    return;
                }
                if (Date < DateTime.Now && Time < DateTime.Now.TimeOfDay.Subtract(TimeSpan.FromMinutes(1)))
                {
                    await dialogService.ShowMessage("Error", "El horario de entrega seleccionado no puede ser anterior al horario actual");

                    return;
                }

                    Notificacion = new Notification();

                    var Hora = "Hora";

                    if (int.Parse(Time.Hours.ToString(culture)) < 10)
                    {
                        Hora = string.Format("0{0}", Time.Hours.ToString(culture));
                    }
                    else
                    {
                        Hora = Time.Hours.ToString(culture);
                    }

                    var Minutos = "Minutos";

                    if (int.Parse(Time.Minutes.ToString(culture)) < 10)
                    {
                        Minutos = string.Format("0{0}", Time.Minutes.ToString(culture));
                    }
                    else
                    {
                        Minutos = Time.Minutes.ToString(culture);
                    }

                    Notificacion.HorarioString = string.Format("{0}:{1} hs", Hora, Minutos);

                    var FechayTiempo = Date.Date + Time;

                    Notificacion.Fecha = Date.Date;

                    Notificacion.Horario = Time;

                    Notificacion.TiempoRestanteEnvio = FechayTiempo - DateTime.Now;

                    var tiempoSchedule = Notificacion.TiempoRestanteEnvio.TotalMinutes;

                    if (string.IsNullOrEmpty(TituloNotification) || string.IsNullOrWhiteSpace(TituloNotification))
                    {
                    Notificacion.Title = "Aviso";
                    }
                    else
                    {
                    Notificacion.Title = string.Format("{0}{1}", TituloNotification.Substring(0, 1).ToUpper(), TituloNotification.Substring(1)); ;
                    }

                    Notificacion.Message = MensajeNotification;

                    Notificacion.TodosLosDiasActivado = true;

                    Notificacion.TodosLosDias = Toggled;

                    ListaNotifications.Add(Notificacion);


                    if (Toggled.Equals(true))
                    {

                    Notificacion.FechaString = "Mensaje diario";

                    Notificacion.IsVisible = true;

                    Notificacion.Toggled = true;

                    for (int i = 1; i <= int.Parse(Text); i++)
                    {
                        notificacionDiaria = new NotificacionDiaria();

                        notificacionDiaria.Title = Notificacion.Title;
                        notificacionDiaria.Message = Notificacion.Message;

                        ListaNotificacionDiaria.Add(notificacionDiaria);



                        CrossLocalNotifications.Current.Show(
                        notificacionDiaria.Title,
                        notificacionDiaria.Message,
                        notificacionDiaria.NotificacionDiariaId,
                        DateTime.Now.AddMinutes(tiempoSchedule).AddDays(i));
                    }
                    Notificacion.ListaNotificacionDiaria = ListaNotificacionDiaria;
                }
                    else
                    {
                        Notificacion.FechaString = string.Format("{0}/{1}/{2}", Date.ToString("dd", culture), Date.ToString("MMM", culture), Date.ToString("yyyy", culture));

                        Notificacion.IsVisible = false;

                        CrossLocalNotifications.Current.Show(
                    Notificacion.Title,
                    Notificacion.Message,
                    Notificacion.NotificationId,
                    DateTime.Now.AddMinutes(tiempoSchedule));
                    }
          
                dataService.Save(ListaNotifications, true);
                dataService.Save(ListaNotificacionDiaria,false);
                Toggled = false;
                TituloNotification = null;
                MensajeNotification = null;
                Date = DateTime.Now;
                Time = DateTime.Now.TimeOfDay;
                CollectionNotification = new ObservableCollection<Notification>(
    ListaNotifications.OrderByDescending(
        x => x.TiempoRestanteEnvio.TotalMinutes));

            }
            catch (Exception e)
            {
                await dialogService.ShowMessage("Error", e.Message);
            }
        }
        public ICommand ToggledCommand
        {
            get
            {
                return new RelayCommand(ToggledAction);
            }
        }

        private void ToggledAction()
        {
            IsVisible = !IsVisible;
            IsVisibleEntry = !IsVisibleEntry;
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
            IsVisible = true;
            Date = DateTime.Now;
            MinimuDate = DateTime.Now;
            Time = DateTime.Now.TimeOfDay;
            ListaNotifications = new List<Notification>();
            ListaNotificacionDiaria = new List<NotificacionDiaria>();
            if (dataService.CheckTableIsEmpty<Notification>())
            {
                ListaNotifications = dataService.Get<Notification>(true);
                foreach (var notification in ListaNotifications)
                {
                    var fechaTiempo = notification.Fecha.Date + notification.Horario;

                    if (fechaTiempo <= DateTime.Now && notification.TodosLosDias.Equals(false))
                    {
                        dataService.Delete(notification);
                    }
                    if (notification.ListaNotificacionDiaria.Count.Equals(0))
                    {
                        dataService.Delete(notification);
                    }  
                }
                ListaNotifications = dataService.Get<Notification>(true);
                try
                {
                    CollectionNotification = new ObservableCollection<Notification>(
        ListaNotifications.OrderByDescending(
            x => x.TiempoRestanteEnvio.TotalMinutes));
                }
                catch
                {

                }
            }

        }

        public async Task Editar(Notification notification)
        {
            try
            {
                if (notification.TodosLosDias.Equals(true))
                {
                    var listaAntigua = notification.ListaNotificacionDiaria.Where(x=>x.notification.NotificationId.Equals(notification.NotificationId)).ToList();

                    var FechayTiempo = Date.Date + Time;

                    notification.TiempoRestanteEnvio = FechayTiempo - DateTime.Now;

                    var tiempoSchedule = notification.TiempoRestanteEnvio.TotalMinutes;

                    foreach (var notif in listaAntigua)
                    {
                        var contador = 1;

                        notif.Title = notification.Title;
                        notif.Message = notification.Message;
     


                        CrossLocalNotifications.Current.Cancel(notif.NotificacionDiariaId);
                        CrossLocalNotifications.Current.Show(
                             notif.Title,
                             notif.Message,
                             notif.NotificacionDiariaId,
                             DateTime.Now.AddMinutes(tiempoSchedule).AddDays(contador));
                        contador++;
                    }

                    dataService.Delete(notification);
                    dataService.Insert(notification, true);
                    dataService.Save(notification.ListaNotificacionDiaria,true);
                }
                else
                {
                    var notificacionAntigua = ListaNotifications.Find(x => x.NotificationId.Equals(notification.NotificationId));

                    notificacionAntigua = notification;

                    dataService.Update(notificacionAntigua, true);

                    var FechayTiempo = Date.Date + Time;

                    notification.TiempoRestanteEnvio = FechayTiempo - DateTime.Now;

                    var tiempoSchedule = notification.TiempoRestanteEnvio.TotalMinutes;


                    CrossLocalNotifications.Current.Cancel(notification.NotificationId);
                    CrossLocalNotifications.Current.Show(
                         Notificacion.Title,
                         Notificacion.Message,
                         Notificacion.NotificationId,
                         DateTime.Now.AddMinutes(tiempoSchedule));
                }
            }
            catch (Exception e)
            {
                await dialogService.ShowMessage("Error", e.Message);
            }

            CollectionNotification = new ObservableCollection<Notification>(ListaNotifications.OrderByDescending(x => x.TiempoRestanteEnvio.TotalMinutes));
        }

        public async Task Delete(Notification notification)
        {
            var confirmacion = false;

            if (Toggled.Equals(false))
            { 
             confirmacion = await dialogService.ShowMessageConfirmacion("Mensaje", "Desea borrar este elemento?");
            }
            else
            {
                confirmacion = true;
            }
            if (confirmacion)
            {

                if (notification.TodosLosDias.Equals(true))
                {
                    var listaAntigua = notification.ListaNotificacionDiaria.Where(x => x.notification.NotificationId.Equals(notification.NotificationId)).ToList();

                    foreach (var notif in listaAntigua)
                    {
                        CrossLocalNotifications.Current.Cancel(notif.NotificacionDiariaId);

                        dataService.Delete(notification);

                        ListaNotifications.Remove(notification);
                    }
                }
                else
                {
                    var notificacionAntigua = ListaNotifications.Find(x => x.NotificationId.Equals(notification.NotificationId));

                    notificacionAntigua = notification;

                    CrossLocalNotifications.Current.Cancel(notification.NotificationId);

                    dataService.Delete(notificacionAntigua);

                    ListaNotifications.Remove(notification);
                }





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
