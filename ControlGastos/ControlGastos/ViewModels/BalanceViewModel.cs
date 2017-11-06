using ControlGastos.Models;
using ControlGastos.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ControlGastos.ViewModels
{
    public class BalanceViewModel:INotifyPropertyChanged
    {
        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Services
        DataService dataService;
        #endregion

        #region Propiedades y Atributos
        public DateTime Date { get; set; }
        public ObservableCollection<string> Meses { get; set; }
        public ObservableCollection<string> Años { get; set; }
        public List<Balance> ListaBalance { get; set; }
        public string SelectedItemAño { get; set; }
        public string SelectedItemMes { get; set; }
        Balance Balance { get; set; }
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

        string _balanceTotal;
        public string BalanceTotal
        {
            get
            {
                return _balanceTotal;
            }
            set
            {
                if (_balanceTotal != value)
                {
                    _balanceTotal = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(BalanceTotal)));
                }
            }
        }

        ObservableCollection<Balance> _collectionBalance;
        public ObservableCollection<Balance> CollectionBalance
        {
            get
            {
                return _collectionBalance;
            }
            set
            {
                if (_collectionBalance != value)
                {
                    _collectionBalance = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CollectionBalance)));
                }
            }
        }


        Color _colorBalance;
        public Color ColorBalance
        {
            get
            {
                return _colorBalance;
            }
            set
            {
                if (_colorBalance != value)
                {
                    _colorBalance = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ColorBalance)));
                }
            }
        }
        #endregion

        #region Commands


        public ICommand SelectedItemMesesCommand
        {
            get
            {
                return new RelayCommand(SelectedItemMeses);
            }
        }

        private void SelectedItemMeses()
        {
            CargarLaObservableCollection(SelectedItemMes,SelectedItemAño);
        }
        public ICommand SelectedItemAñosCommand
        {
            get
            {
                return new RelayCommand(SelectedItemAños);
            }
        }

        private void SelectedItemAños()
        {
            CargarLaObservableCollection(SelectedItemMes,SelectedItemAño);
        }
        #endregion

        #region Constructor
        public BalanceViewModel()
        {
            dataService = new DataService();

            Cargas();

        }
        #endregion
      
        #region Métodos
        private void Cargas()
        {
            ListaBalance = new List<Balance>();
           
            //Item Source del Picker de Meses
            Meses = new ObservableCollection<string>();
            //Item Source del Picker de Años
            Años = new ObservableCollection<string>();
           
            //Agregado de meses al picker dependiendo si existen o no datos guardados en esos meses
            if (dataService.CheckTableIsEmpty<Ingresos>() || dataService.CheckTableIsEmpty<Gastos>())
            {
                foreach (var MesesIngresos in dataService.Get<Ingresos>(true))
                {
                    if (!string.IsNullOrEmpty(MesesIngresos.Mes))
                    {
                        if (Meses.Contains(MesesIngresos.Mes) == false)
                        {
                            Meses.Add(MesesIngresos.Mes);
                        }
                    }
                }
                foreach (var MesesGastos in dataService.Get<Gastos>(true))
                {
                    if (!string.IsNullOrEmpty(MesesGastos.Mes))
                    {
                        if (Meses.Contains(MesesGastos.Mes) == false)
                        {
                            Meses.Add(MesesGastos.Mes);
                        }
                    }
                }
              
            }
            //Agregado de años al picker dependiendo si existen o no datos guardados en esos años

            //CheckTableIsEmpty<Ingresos>() comprueba si hay algo en la tabla, si no hay es false, si hay es true
            if (dataService.CheckTableIsEmpty<Ingresos>() || dataService.CheckTableIsEmpty<Gastos>())
            {
                foreach (var AñosIngresos in dataService.Get<Ingresos>(true))
                {
                    if (!string.IsNullOrEmpty(AñosIngresos.Mes))
                    {
                        if (Años.Contains(AñosIngresos.Anio) == false)
                        {
                            Años.Add(AñosIngresos.Anio);
                        }
                    }
                }
                foreach (var AñosGastos in dataService.Get<Gastos>(true))
                {
                    if (!string.IsNullOrEmpty(AñosGastos.Anio))
                    {
                        if (Años.Contains(AñosGastos.Anio) == false)
                        {
                            Años.Add(AñosGastos.Anio);
                        }
                    }
                }
            }

            foreach (var meses in Meses)
            {
                if (meses == DateTime.Now.ToString("MMMM"))
                {
                    foreach (var años in Años)
                    {
                        if (años == DateTime.Now.ToString("yyyy"))
                        {
                  
                            CargarLaObservableCollection(meses, años);
                        }
                    }
                }
            }



        }

        private void CargarLaObservableCollection(string selectedItemMes, string selectedItemAño)
        {
            ListaBalance.Clear();
            if (dataService.CheckTableIsEmpty<Ingresos>() || dataService.CheckTableIsEmpty<Gastos>())
            {
                //Agregado de objetos balance a las observable collection en caso de que se encuentren el mes y el año pertenecientes a la fecha del día. En función de los ingresos 
                if (dataService.Get<Ingresos>(true).Exists(x => x.Mes == selectedItemMes && x.Anio == selectedItemAño))
                {
                    foreach (var ListadeBalanceIngreso in dataService.Get<Ingresos>(true).Where(x => x.Mes == selectedItemMes && x.Anio == selectedItemAño).ToList())
                    {
                        Balance = new Balance
                        {
                            Fecha = string.Format("{0}/{1}/{2}", ListadeBalanceIngreso.Dia, ListadeBalanceIngreso.Mes, ListadeBalanceIngreso.Anio),
                            GastoIngreso="Ingreso",
                            Cantidad = ListadeBalanceIngreso.IngresoCantidad,
                            ColorGastoIngreso=Color.Green,
                            Origen = ListadeBalanceIngreso.IngresoNombre
                        };
                        ListaBalance.Add(Balance);
                    }
                }
                //Agregado de objetos balance a las observable collection en caso de que se encuentren el mes y el año pertenecientes a la fecha del día. En función de los gastos 
                if (dataService.Get<Gastos>(true).Exists(x => x.Mes == selectedItemMes && x.Anio == selectedItemAño))
                {
                    foreach (var ListadeBalanceGastos in dataService.Get<Gastos>(true).Where(x => x.Mes == selectedItemMes && x.Anio == selectedItemAño).ToList())
                    {
                        Balance = new Balance
                        {
                            Fecha = string.Format("{0}/{1}/{2}", ListadeBalanceGastos.Dia, ListadeBalanceGastos.Mes, ListadeBalanceGastos.Anio),
                            GastoIngreso=string.Format("Gasto: {0}",ListadeBalanceGastos.Categoria),
                            Cantidad = string.Format("-{0}",ListadeBalanceGastos.GastosCantidad),
                            ColorGastoIngreso=Color.Red,
                            Origen = ListadeBalanceGastos.GastoNombre
                        };
                        ListaBalance.Add(Balance);
                    }
                }
                CollectionBalance = new ObservableCollection<Balance>(ListaBalance.OrderBy(x => int.Parse(x.Fecha.Substring(0, 2))).ToList());
             
                BalanceTotal = ListaBalance.Sum(x => int.Parse(x.Cantidad)).ToString();
                if (int.Parse(BalanceTotal) < 0)
                {
                    ColorBalance = Color.Red;
                }
                else if(int.Parse(BalanceTotal) > 0)
                {
                    ColorBalance = Color.Green;
                }
            }
        }
        #endregion


    }
}
