using ControlGastos.Behaviors;
using ControlGastos.Models;
using ControlGastos.Services;
using GalaSoft.MvvmLight.Command;
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
    public class IngresosViewModel:INotifyPropertyChanged
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
       
        public DateTime Date { get; set; }
        Ingresos Ingresos { get; set; }
       public List<Ingresos> ListaIngresos { get; set; }
        string _sumaIngreso;
        public string SumaIngreso
        {
            get
            {
                return _sumaIngreso;
            }
            set
            {
                if (_sumaIngreso != value)
                {
                    _sumaIngreso = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SumaIngreso)));
                }
            }
        }

        string _montoIngreso;
        public string MontoIngreso
    {
            get
            {
                return _montoIngreso;
            }
            set
            {
                if (_montoIngreso != value)
                {
                    _montoIngreso = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MontoIngreso)));
                }
            }
        }
        ObservableCollection<Ingresos> _collectionIngresos;
        public ObservableCollection<Ingresos> CollectionIngresos
        {
            get
            {
                return _collectionIngresos;
            }
            set
            {
                if (_collectionIngresos != value)
                {
                    _collectionIngresos = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CollectionIngresos)));
                }
            }
        }
        public string _mes;
        public string Mes
        {
            get
            {
                return _mes;
            }
            set
            {
                if (_mes != value)
                {
                    _mes = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Mes)));
                }
            }
        }

        string _origenIngreso;
        public string OrigenIngreso
        {
            get
            {
                return _origenIngreso;
            }
            set
            {
                if (_origenIngreso != value)
                {
                    _origenIngreso = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OrigenIngreso)));
                }
            }
        }
        #endregion

        #region Commands

        public ICommand AgregarIngresoCommand
        {
            get
            {
                return new RelayCommand(AgregarIngreso);
            }
        }

        private async void AgregarIngreso()
        {

                //Crea el objeto Ingreso, lo agrego a la lista del mes, y después se hace la sumatoria de la lista

                if (MontoIngreso == "0"||string.IsNullOrEmpty(MontoIngreso) ||string.IsNullOrWhiteSpace(MontoIngreso))
                {
                    await dialogService.ShowMessage("Error", "Debe asignar un valor mayor que cero");
                    return;
                }

                Ingresos = new Ingresos();
                Ingresos.Anio = Date.ToString("yyyy", culture);
                Ingresos.Mes = Date.ToString("MMM", culture);
                Ingresos.Dia = Date.ToString("dd", culture);
                Ingresos.ImagenFecha = "date";
                if (string.IsNullOrEmpty(OrigenIngreso))
                {
                    OrigenIngreso = "Sin Origen";
                }
                Ingresos.IngresoNombre = string.Format("{0}{1}", OrigenIngreso.Substring(0, 1).ToUpper(), OrigenIngreso.Substring(1));
                Ingresos.ImagenOrigen = "income";
                if (MontoIngreso == null)
                {
                    MontoIngreso = 0.ToString();
                }
                if (!double.TryParse(MontoIngreso, out double result))
                {
                    await dialogService.ShowMessage("Error", "El contenido del monto debe ser un número");
                    MontoIngreso = null;
                    return;
                }
                if (MontoIngreso.Contains("-"))
                {
                    Ingresos.IngresoCantidad = MontoIngreso.Replace("-", "");
                }
                else
                {
                    Ingresos.IngresoCantidad = string.Format("{0}", MontoIngreso);
                }
                Ingresos.ImagenMonto = "money";
                ListaIngresos.Add(Ingresos);
                //Realizar la sumatoria con los ingresos pertenecientes al mes y año elegido
                SumaIngreso = ListaIngresos.Where(x => x.Mes == Ingresos.Mes && x.Anio == Ingresos.Anio).ToList().Sum(x => double.Parse(x.IngresoCantidad)).ToString();
                MontoIngreso = null;
                OrigenIngreso = null;
                dataService.Save(ListaIngresos, true);
                CollectionIngresos = new ObservableCollection<Ingresos>(ListaIngresos.Where(x => x.Mes == Ingresos.Mes && x.Anio == Ingresos.Anio).ToList());

        }
        public ICommand DateSelectedCommand
        {
            get
            {
                return new RelayCommand(DateSelected);
            }
        }

        private void DateSelected()
        {
            Mes = Date.ToString("MMMM",culture);
            SumaIngreso = ListaIngresos.Where(x => x.Mes == Date.ToString("MMM", culture) && 
            x.Anio == Date.ToString("yyyy", culture)).ToList().Sum(x => double.Parse(x.IngresoCantidad)).ToString();
            CollectionIngresos = new ObservableCollection<Ingresos>(ListaIngresos.Where(x => x.Mes == Date.ToString("MMM", culture) && x.Anio == Date.ToString("yyyy", culture)).ToList());
        }
        #endregion

        #region Constructor
        public IngresosViewModel()
        {
            dataService = new DataService();
            dialogService = new DialogService();
            culture = new CultureInfo("es-ES");
            instance = this;

            Cargas();

        }
        #endregion

        #region Singleton

        static IngresosViewModel instance;

        public static IngresosViewModel GetInstance()
        {
            if (instance == null)
            {
                return new IngresosViewModel();
            }
            return instance;
        }
        #endregion

        #region Métodos
        private void Cargas()
        {
            Mes = DateTime.Now.ToString("MMMM", culture);
            Date = DateTime.Now;
            ListaIngresos = new List<Ingresos>();
            if (dataService.CheckTableIsEmpty<Ingresos>())
            {
               ListaIngresos= dataService.Get<Ingresos>(true);
               SumaIngreso = ListaIngresos.Where(x => x.Mes == Date.ToString("MMM", culture) && 
               x.Anio == Date.ToString("yyyy", culture)).ToList().Sum(x => double.Parse(x.IngresoCantidad)).ToString();
               CollectionIngresos = new ObservableCollection<Ingresos>(ListaIngresos.Where(x => x.Mes == Date.ToString("MMM", culture) && 
               x.Anio == Date.ToString("yyyy", culture)).ToList());
            }
            else
            {
                SumaIngreso = "0";
            }
        }

        public void Editar(Ingresos ingresos)
        {
            var ingresoAntiguo = ListaIngresos.Find(x => x.IngresoId == ingresos.IngresoId);
            ingresoAntiguo = ingresos;
            dataService.Update(ingresoAntiguo, true);
            SumaIngreso = ListaIngresos.Where(x => x.Mes == Date.ToString("MMM", culture) &&
                x.Anio == Date.ToString("yyyy", culture)).ToList().Sum(x => double.Parse(x.IngresoCantidad)).ToString();
            CollectionIngresos = new ObservableCollection<Ingresos>(ListaIngresos.OrderByDescending(x => double.Parse(x.Dia)).ToList());
        }

        public async void Delete(Ingresos ingresos)
        {
            var confirmacion = await dialogService.ShowMessageConfirmacion("Mensaje", "Desea borrar este elemento?");

            if (confirmacion)
            {

                var ingresoAntiguo = ListaIngresos.Find(x => x.IngresoId == ingresos.IngresoId);
                dataService.Delete(ingresoAntiguo);
                ListaIngresos.Remove(ingresoAntiguo);
                SumaIngreso = ListaIngresos.Where(x => x.Mes == Date.ToString("MMM", culture) &&
                x.Anio == Date.ToString("yyyy", culture)).ToList().Sum(x => double.Parse(x.IngresoCantidad)).ToString();

                CollectionIngresos = new ObservableCollection<Ingresos>(ListaIngresos.OrderByDescending(x => double.Parse(x.Dia)).ToList());
            }
            else
            {
                return;
            }
        }
        #endregion

    }
}
