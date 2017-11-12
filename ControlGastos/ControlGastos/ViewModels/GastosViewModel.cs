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
    public class GastosViewModel:INotifyPropertyChanged
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
        public string OrigenGasto { get; set; }
        public string SelectedItem { get; set; }
        Gastos Gastos { get; set; }
        public List<Gastos> ListaGastos { get; set; }
        public ObservableCollection<string> PickerCategorias { get; set; }
        string _sumaGastoCategoria;
        public string SumaGastoCategoria
        {
            get
            {
                return _sumaGastoCategoria;
            }
            set
            {
                if (_sumaGastoCategoria != value)
                {
                    _sumaGastoCategoria = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SumaGastoCategoria)));
                }
            }
        }
        string _sumaGasto;
        public string SumaGasto
        {
            get
            {
                return _sumaGasto;
            }
            set
            {
                if (_sumaGasto != value)
                {
                    _sumaGasto = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SumaGasto)));
                }
            }
        }
        string _montoGasto;
        public string MontoGasto
        {
            get
            {
                return _montoGasto;
            }
            set
            {
                if (_montoGasto != value)
                {
                    _montoGasto = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MontoGasto)));
                }
            }
        }
        public string _categoria;
        public string Categoria
        {
            get
            {
                return _categoria;
            }
            set
            {
                if (_categoria != value)
                {
                    _categoria = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Categoria)));
                }
            }
        }
        ObservableCollection<Gastos> _collectionGastos;
        public ObservableCollection<Gastos> CollectionGastos
        {
            get
            {
                return _collectionGastos;
            }
            set
            {
                if (_collectionGastos != value)
                {
                    _collectionGastos = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CollectionGastos)));
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
        #endregion

        #region Commands

        public ICommand AgregarGastoCommand
        {
            get
            {
                return new RelayCommand(AgregarGasto);
            }
        }

        private async void AgregarGasto()
        {
            //Crea el objeto Ingreso, lo agrego a la lista del mes, y después se hace la sumatoria de la lista
           
            if (SelectedItem == null)
            {
               await dialogService.ShowMessage("Error", 
                   "Se debe seleccionar una categoría");
                return;
            }
            Gastos = new Gastos();
            Gastos.Anio = Date.ToString("yyyy",culture);
            Gastos.Mes = Date.ToString("MMM",culture);
            Gastos.Dia = Date.ToString("dd",culture);
            Gastos.ImagenFecha = "date";
            if (string.IsNullOrEmpty(OrigenGasto))
            {
                OrigenGasto = "Sin Origen";
            }

            Gastos.Categoria = SelectedItem;
            Gastos.GastoNombre = OrigenGasto;
            switch (Gastos.Categoria)
            {
                case "Servicios":
                    Gastos.ImagenOrigen = "servicios";
                    break;
                case "Ocio":
                    Gastos.ImagenOrigen = "ocio";
                    break;
                case "Provisiones":
                    Gastos.ImagenOrigen = "provisiones";
                    break;
                default:
                    Gastos.ImagenOrigen = "Sin Imagen Disponible";
                    break;
            }
            if (MontoGasto == null)
            {
                MontoGasto = 0.ToString();
            }
            if(!double.TryParse(MontoGasto,out double result))
            {
                await dialogService.ShowMessage("Error", "El contenido del monto debe ser un número");
                MontoGasto = null;
                return;
            }
            if (!MontoGasto.Contains("-"))
            { 
            Gastos.GastosCantidad =string.Format("-{0}",MontoGasto);
            }
            else
            {
                Gastos.GastosCantidad = string.Format("{0}", MontoGasto);
            }
            Gastos.ImagenMonto = "money";
            ListaGastos.Add(Gastos);
            //Realizar la sumatoria con los ingresos pertenecientes al mes y año elegido
            SumaGasto = ListaGastos.Where(x => x.Mes == Gastos.Mes && x.Anio == Gastos.Anio).ToList().Sum(x => double.Parse(x.GastosCantidad)).ToString();
            SumaGastoCategoria = ListaGastos.Where(x => x.Mes == Gastos.Mes && x.Anio == Gastos.Anio&&x.Categoria== SelectedItem).ToList().Sum(x => double.Parse(x.GastosCantidad)).ToString();
            MontoGasto = null;
            OrigenGasto = null;
            dataService.Save(ListaGastos, true);
            CollectionGastos = new ObservableCollection<Gastos>(ListaGastos.Where(x => x.Mes == Gastos.Mes && x.Anio == Gastos.Anio && x.Categoria == SelectedItem).ToList());
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
            Mes = Date.ToString("MMMM");
            SumaGasto = ListaGastos.Where(x => x.Mes == Date.ToString("MMM",culture) &&
            x.Anio == Date.ToString("yyyy",culture)).ToList().Sum(x => double.Parse(x.GastosCantidad)).ToString();
            SumaGastoCategoria = ListaGastos.Where(x => x.Mes == Date.ToString("MMM", culture) && 
            x.Anio == Date.ToString("yyyy", culture) && 
            x.Categoria == SelectedItem).ToList().Sum(x => int.Parse(x.GastosCantidad)).ToString();
            CollectionGastos = new ObservableCollection<Gastos>(ListaGastos.Where(x => x.Mes == Date.ToString("MMM", culture) &&
            x.Anio == Date.ToString("yyyy", culture) && 
            x.Categoria == SelectedItem).ToList());
        }
        public ICommand SelectedItemChangedCommand
        {
            get
            {
                return new RelayCommand(SelectedItemChanged);
            }
        }

        private void SelectedItemChanged()
        {
            Categoria = SelectedItem;
            SumaGastoCategoria = ListaGastos.Where(x => x.Mes == Date.ToString("MMM",culture) &&
            x.Anio == Date.ToString("yyyy",culture) && 
            x.Categoria == SelectedItem).ToList().Sum(x => int.Parse(x.GastosCantidad)).ToString();
            CollectionGastos = new ObservableCollection<Gastos>(ListaGastos.Where(x => x.Mes == Date.ToString("MMM", culture) &&
            x.Anio == Date.ToString("yyyy", culture) &&
            x.Categoria == SelectedItem).ToList());
        }
        #endregion

        #region Constructor
        public GastosViewModel()
        {
            culture = new CultureInfo("es-ES");
            dataService = new DataService();
            dialogService = new DialogService();
            Cargas();

        }
        #endregion

        #region Métodos
        private void Cargas()
        {
         
            Categoria = "SIN SELECCIONAR";
            PickerCategorias = new ObservableCollection<string>();
            PickerCategorias.Add("Ocio");
            PickerCategorias.Add("Servicios");
            PickerCategorias.Add("Provisiones");
            Mes = DateTime.Now.ToString("MMMM",culture);
            Date = DateTime.Now;
            ListaGastos = new List<Gastos>();
            
            if (dataService.CheckTableIsEmpty<Gastos>())
            {
                ListaGastos = dataService.Get<Gastos>(true);
                SumaGastoCategoria = ListaGastos.Where(x => x.Mes == Date.ToString("MMM",culture) && 
                x.Anio == Date.ToString("yyyy",culture)&&
                x.Categoria==SelectedItem).ToList().Sum(x => double.Parse(x.GastosCantidad)).ToString();
                SumaGasto = ListaGastos.Where(x => x.Mes == Date.ToString("MMM",culture) &&
                x.Anio == Date.ToString("yyyy",culture)).ToList().Sum(x => double.Parse(x.GastosCantidad)).ToString();
                CollectionGastos = new ObservableCollection<Gastos>(ListaGastos.Where(x => x.Mes == Date.ToString("MMM", culture) &&
                x.Anio == Date.ToString("yyyy", culture) &&
                x.Categoria == SelectedItem).ToList());
            }
            else
            {
              
                 SumaGasto = "0";
               
            }
        }

        }
        #endregion

    
}

