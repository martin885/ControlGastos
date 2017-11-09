using ControlGastos.Behaviors;
using ControlGastos.Models;
using ControlGastos.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
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
        public string Mes { get; set; }
        public DateTime Date { get; set; }
        public string OrigenIngreso { get; set; }
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
            
            Ingresos = new Ingresos();
            culture = new CultureInfo("es-ES");
            Ingresos.Anio = Date.ToString("yyyy",culture);
            Ingresos.Mes= Date.ToString("MMMM", culture);
            Ingresos.Dia = Date.ToString("dd", culture);
            if (string.IsNullOrEmpty(OrigenIngreso))
            {
                OrigenIngreso = "Sin Origen";
            }
            Ingresos.IngresoNombre = OrigenIngreso;
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
            ListaIngresos.Add(Ingresos);
            //Realizar la sumatoria con los ingresos pertenecientes al mes y año elegido
            SumaIngreso = ListaIngresos.Where(x=>x.Mes== Ingresos.Mes && x.Anio == Ingresos.Anio).ToList().Sum(x => double.Parse(x.IngresoCantidad)).ToString();
            MontoIngreso = null;
            dataService.Save(ListaIngresos, true);
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
            SumaIngreso = ListaIngresos.Where(x => x.Mes == Date.ToString("MMMM", culture) && x.Anio == Date.ToString("yyyy", culture)).ToList().Sum(x => double.Parse(x.IngresoCantidad)).ToString();
        }
        #endregion

        #region Constructor
        public IngresosViewModel()
        {
            dataService = new DataService();
            dialogService = new DialogService();

            Cargas();

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
               SumaIngreso = ListaIngresos.Where(x => x.Mes == Date.ToString("MMMM", culture) && x.Anio == Date.ToString("yyyy", culture)).ToList().Sum(x => double.Parse(x.IngresoCantidad)).ToString();
            }
            else
            {
                SumaIngreso = "0";
            }
        }
        #endregion

    }
}
