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
        DialogService dialogService;
        #endregion

        #region Propiedades y Atributos
        IFormatProvider culture;
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
            culture = new CultureInfo("es-ES");
            instance = this;
            dataService = new DataService();
            dialogService = new DialogService();

            Cargas();

        }
        #endregion

        #region Singleton

        static BalanceViewModel instance;

        public static BalanceViewModel GetInstance()
        {
            if (instance == null)
            {
                return new BalanceViewModel();
            }
            return instance;
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
              
          
            //Agregado de años al picker dependiendo si existen o no datos guardados en esos años


         
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
            

            foreach (var meses in Meses)
            {
                if (meses == DateTime.Now.ToString("MMMM",culture))
                {
                    foreach (var años in Años)
                    {
                        if (años == DateTime.Now.ToString("yyyy",culture))
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
        
                //Agregado de objetos balance a las observable collection en caso de que se encuentren el mes y el año pertenecientes a la fecha del día. En función de los ingresos 
                if (dataService.Get<Ingresos>(true).Exists(x => x.Mes == selectedItemMes && x.Anio == selectedItemAño))
                {
                    foreach (var ListadeBalanceIngreso in dataService.Get<Ingresos>(true).Where(x => x.Mes == selectedItemMes && x.Anio == selectedItemAño).ToList())
                    {
                        Balance = new Balance
                        {
                            BalanceId=ListadeBalanceIngreso.IngresoId.ToString(),
                            Dia = ListadeBalanceIngreso.Dia,
                            Mes = ListadeBalanceIngreso.Mes,
                            Anio = ListadeBalanceIngreso.Anio,
                            Fecha = string.Format("{0}/{1}/{2}", ListadeBalanceIngreso.Dia, ListadeBalanceIngreso.Mes, ListadeBalanceIngreso.Anio),
                            GastoIngreso="Ingreso",
                            Cantidad = ListadeBalanceIngreso.IngresoCantidad,
                            ColorGastoIngreso=Color.Green,
                            Origen = ListadeBalanceIngreso.IngresoNombre
                        };
                        if (!double.TryParse(Balance.Cantidad, out double result))
                        {
                            Balance.Cantidad = 0.ToString();
                        }
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
                            BalanceId = ListadeBalanceGastos.GastosId.ToString(),
                            Dia= ListadeBalanceGastos.Dia,
                            Mes= ListadeBalanceGastos.Mes,
                            Anio= ListadeBalanceGastos.Anio,
                            Fecha = string.Format("{0}/{1}/{2}",ListadeBalanceGastos.Dia, ListadeBalanceGastos.Mes, ListadeBalanceGastos.Anio),
                            GastoIngreso=string.Format("Gasto: {0}",ListadeBalanceGastos.Categoria),
                            Cantidad = string.Format("-{0}",ListadeBalanceGastos.GastosCantidad),
                            ColorGastoIngreso=Color.Red,
                            Origen = ListadeBalanceGastos.GastoNombre
                        };
                        if (Balance.Cantidad.Contains("--"))
                        {
                            Balance.Cantidad = Balance.Cantidad.Replace("--", "-");
                        }
                        if (!double.TryParse(Balance.Cantidad, out double result))
                        {
                            Balance.Cantidad = 0.ToString();     
                        }
                        ListaBalance.Add(Balance);
                    
                    }
                }
                CollectionBalance = new ObservableCollection<Balance>(ListaBalance.OrderBy(x => double.Parse(x.Fecha.Substring(0, 2))).ToList());

            

                BalanceTotal = ListaBalance.Sum(x => double.Parse(x.Cantidad)).ToString();
                if (double.Parse(BalanceTotal) < 0)
                {
                    ColorBalance = Color.Red;
                }
                else if(double.Parse(BalanceTotal) > 0)
                {
                    ColorBalance = Color.Green;
                }
            

        }

        public void Editar(Balance balance)
        {
            var balanceAntiguo = ListaBalance.Find(x => x.GastoIngreso == balance.GastoIngreso && x.BalanceId == balance.BalanceId);
            if (balance.GastoIngreso == "Ingreso")
            {
               var IngresoAntiguo= dataService.Get<Ingresos>(true).Find(x => x.IngresoId.ToString() == balance.BalanceId);
                IngresoAntiguo.Dia = balance.Dia;
                IngresoAntiguo.Mes = balance.Mes;
                IngresoAntiguo.Anio = balance.Anio;
                IngresoAntiguo.IngresoNombre = balance.Origen;
                IngresoAntiguo.IngresoCantidad = balance.Cantidad;
                dataService.Update(IngresoAntiguo, true);
            }
            else
            {
                var GastoAntiguo = dataService.Get<Gastos>(true).Find(x => x.GastosId.ToString() == balance.BalanceId);
                GastoAntiguo.Dia = balance.Dia;
                GastoAntiguo.Mes = balance.Mes;
                GastoAntiguo.Anio = balance.Anio;
                GastoAntiguo.GastoNombre = balance.Origen;
                GastoAntiguo.GastosCantidad = balance.Cantidad;
                dataService.Update(GastoAntiguo, true);
            }
            balanceAntiguo = balance;
            CollectionBalance = new ObservableCollection<Balance>(ListaBalance.OrderBy(x => double.Parse(x.Fecha.Substring(0, 2))).ToList());
        }
        public async void Delete(Balance balance)
        {
            var confirmacion = await dialogService.ShowMessageConfirmacion("Mensaje", "Desea borrar este elemento");
            SelectedItemMes = balance.Mes;
            SelectedItemAño = balance.Anio;
            if (confirmacion)
            {
                if (balance.GastoIngreso == "Ingreso")
                {
                    var IngresoAntiguo = dataService.Get<Ingresos>(true).Find(x => x.IngresoId.ToString() == balance.BalanceId);
                    dataService.Delete(IngresoAntiguo);
                }
               else 
                {
                    var GastoAntiguo = dataService.Get<Gastos>(true).Find(x => x.GastosId.ToString() == balance.BalanceId);
                    dataService.Delete(GastoAntiguo);
                }
              

                CargarLaObservableCollection(SelectedItemMes, SelectedItemAño);
            }
            else
            {
                return;
            }
        }
            #endregion


        }
}
