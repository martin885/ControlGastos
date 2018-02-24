using ControlGastos.Interfaces;
using ControlGastos.Models;
using ControlGastos.Services;
using ControlGastos.Views;
using GalaSoft.MvvmLight.Command;
using Plugin.Messaging;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ControlGastos.ViewModels
{
    public class BalanceViewModel : INotifyPropertyChanged
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
        public string DefaultAño { get; set; }
        public string DefaultMes { get; set; }
        Balance Balance { get; set; }
        public int cont { get; private set; }
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

        public ICommand SelectedItemMesesCommand
        {
            get
            {
                return new RelayCommand(SelectedItemMeses);
            }
        }

        private void SelectedItemMeses()
        {
            CargarLaObservableCollection(SelectedItemMes, SelectedItemAño);
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
            CargarLaObservableCollection(SelectedItemMes, SelectedItemAño);
        }

        public ICommand EmailCommand
        {
            get
            {
                return new RelayCommand(Email);
            }
        }

        private void Email()
        {

            var emailMessenger = CrossMessaging.Current.EmailMessenger;
            if (emailMessenger.CanSendEmail && emailMessenger.CanSendEmailAttachments)
            {
                var filename = string.Format("Balance Mensual de {0}-{1}", SelectedItemMes, SelectedItemAño);
                emailMessenger.SendEmail(DependencyService.Get<IEmail>().EmailMessage(filename));
            }
        }

        public ICommand ExcelCommand
        {
            get
            {
                return new RelayCommand(Excel);
            }
        }

        private async void Excel()
        {
            var balanceView = BalanceView.GetInstance();

            var confirmacion = await dialogService.ShowMessageConfirmacion("Mensaje", "¿Desea exportar el balance a una planilla de cálculo?");

            if (confirmacion)
            {
                Cargas();
                if (ListaBalance.Count == 0)
                {
                    await dialogService.ShowMessage("Error", "Se deben agregar elementos al balance");
                    return;
                }
                using (ExcelEngine excelEngine = new ExcelEngine())
                {

                    cont = 0;
                    //Seleccionar versión de Excel 2013
                    excelEngine.Excel.DefaultVersion = ExcelVersion.Excel2013;

                    //Crear workbook con una hoja de trabajo
                    IWorkbook workbook = excelEngine.Excel.Workbooks.Create(1);

                    //Acceder a la primera hoja de trabajo desde la instancia de workbook
                    IWorksheet worksheet = workbook.Worksheets[0];

                    IMigrantRange migrantRange = worksheet.MigrantRange;

                    foreach (var elemento in ListaBalance)
                    {

                        // Writing Data.
                        //cont aumenta en 7 la posición de las filas en cada producto, las columnas dependen de los días elegidos

                        migrantRange["A1"].Text = "Fecha";
                        migrantRange["A1"].CellStyle.Font.Bold = true;

                        migrantRange["B1"].Text = "Origen";
                        migrantRange["B1"].CellStyle.Font.Bold = true;

                        migrantRange["C1"].Text = "Categoría";
                        migrantRange["C1"].CellStyle.Font.Bold = true;

                        migrantRange["D1"].Text = "Monto";
                        migrantRange["D1"].CellStyle.Font.Bold = true;

                        //Nueva celda
                        migrantRange.ResetRowColumn(cont + 2, 1);
                        migrantRange.Text = string.Format("{0}/{1}/{2}", elemento.Dia, elemento.Mes, elemento.Anio);


                        //migrantRange.CellStyle.Borders.LineStyle = ExcelLineStyle.Medium;

                        //Nueva celda
                        migrantRange.ResetRowColumn(cont + 2, 2);
                        migrantRange.Text = elemento.Origen;
                        //Nueva celda
                        migrantRange.ResetRowColumn(cont + 2, 3);
                        migrantRange.Text = elemento.GastoIngreso;
                        //Nueva celda
                        migrantRange.ResetRowColumn(cont + 2, 4);

                        migrantRange.Number = double.Parse(elemento.Cantidad);
                        if (double.Parse(elemento.Cantidad) > 0)
                        {
                            worksheet[string.Format("D{0}", cont + 2)].CellStyle.Font.Color = ExcelKnownColors.Green;
                        }
                        else if (double.Parse(elemento.Cantidad) < 0)
                        {
                            worksheet[string.Format("D{0}", cont + 2)].CellStyle.Font.Color = ExcelKnownColors.Red;
                        }


                        cont = cont + 1;

                    };

                    IRange range = worksheet.Range[string.Format("A{0}:C{0}", cont + 2)];
                    range.Merge();
                    range.Text = string.Format("Balance: ");
                    range.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    range.CellStyle.Font.Bold = true;
                    worksheet[string.Format("D{0}", cont + 2)].Number = double.Parse(BalanceTotal);
                    worksheet[string.Format("D{0}", cont + 2)].CellStyle.Font.Bold = true;
                    if (double.Parse(BalanceTotal) > 0)
                    {
                        worksheet[string.Format("D{0}", cont + 2)].CellStyle.ColorIndex = ExcelKnownColors.Green;
                    }
                    else if (double.Parse(BalanceTotal) < 0)
                    {
                        worksheet[string.Format("D{0}", cont + 2)].CellStyle.ColorIndex = ExcelKnownColors.Red;
                    }
                    worksheet.Range[string.Format("A1:D{0}", cont + 2)].BorderInside();
                    worksheet.Range[string.Format("A1:D{0}", cont + 2)].BorderAround();
                    worksheet.UsedRange.AutofitColumns();

                    //Save the workbook to stream in xlsx format. 
                    MemoryStream stream = new MemoryStream();
                    workbook.SaveAs(stream);

                    workbook.Close();

                    //Save the stream as a file in the device and invoke it for viewing
                    await DependencyService.Get<ISave>().SaveAndView(string.Format("Balance Mensual de {0}-{1}", SelectedItemMes, SelectedItemAño) + ".xlsx", "application/msexcel", stream);

                    await dialogService.ShowMessage("Mensaje", string.Format("El balance se guardó como archivo de nombre '{0}' en la carpeta Balances", string.Format("Balance Mensual de {0}-{1}", SelectedItemMes, SelectedItemAño) + ".xlsx"));
                }
            }
            balanceView.excelUnTapped();
        }

        public ICommand InfoCommand
        {
            get
            {
                return new RelayCommand(Info);
            }
        }

        private async void Info()
        {
            //Instanciar ViewModel
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.Info = new InfoViewModel();
            //Ir a la página de información
            //var balanceView = BalanceView.GetInstance();
            //await balanceView.Navigation.PushAsync(new InfoView());
            await Application.Current.MainPage.Navigation.PushAsync(new InfoView());
        }

        public ICommand NotificationCommand
        {
            get
            {
                return new RelayCommand(Notification);
            }
        }

        private async void Notification()
        {
            //Instanciar ViewModel
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.Notification = new NotificationViewModel();
            //Ir a la página de notificación
            //var balanceView = BalanceView.GetInstance();
            //await balanceView.Navigation.PushAsync(new NotificationView());
            await Application.Current.MainPage.Navigation.PushAsync(new NotificationView());
        }
        #endregion

        #region Constructor
        public BalanceViewModel()
        {

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
            culture = new CultureInfo("es-ES");
            ListaBalance = new List<Balance>();
            IsRefreshing = true;
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
                if (meses == DateTime.Now.ToString("MMM", culture))
                {
                    foreach (var años in Años)
                    {
                        if (años == DateTime.Now.ToString("yyyy", culture))
                        {
                            CargarLaObservableCollection(meses, años);
                        }
                    }
                }
            }
            DefaultMes = Meses.IndexOf(DateTime.Now.ToString("MMM", culture)).ToString();
            DefaultAño = Años.IndexOf(DateTime.Now.ToString("yyyy", culture)).ToString();
            IsRefreshing = false;
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
                        BalanceId = ListadeBalanceIngreso.IngresoId.ToString(),
                        Dia = ListadeBalanceIngreso.Dia,
                        Mes = ListadeBalanceIngreso.Mes,
                        Anio = ListadeBalanceIngreso.Anio,
                        Fecha = string.Format("{0}/{1}/{2}", ListadeBalanceIngreso.Dia, ListadeBalanceIngreso.Mes, ListadeBalanceIngreso.Anio),
                        ImagenFecha = "date",
                        GastoIngreso = "Ingreso",
                        Cantidad = ListadeBalanceIngreso.IngresoCantidad,
                        ImagenMonto = "money",
                        ColorGastoIngreso = Color.Green,
                        Origen = ListadeBalanceIngreso.IngresoNombre,
                        ImagenOrigen = "income"
                    };
                    if (!double.TryParse((string)Balance.Cantidad, out double result))
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
                        Dia = ListadeBalanceGastos.Dia,
                        Mes = ListadeBalanceGastos.Mes,
                        Anio = ListadeBalanceGastos.Anio,
                        Fecha = string.Format("{0}/{1}/{2}", ListadeBalanceGastos.Dia, ListadeBalanceGastos.Mes, ListadeBalanceGastos.Anio),
                        ImagenFecha = "date",
                        GastoIngreso = ListadeBalanceGastos.Categoria,
                        Cantidad = string.Format("-{0}", ListadeBalanceGastos.GastosCantidad),
                        ImagenMonto = "money",
                        ColorGastoIngreso = Color.Red,
                        Origen = ListadeBalanceGastos.GastoNombre
                    };
                    if (Balance.Cantidad.Contains("--"))
                    {
                        Balance.Cantidad = Balance.Cantidad.Replace("--", "-");
                    }
                    if (!double.TryParse((string)Balance.Cantidad, out double result))
                    {
                        Balance.Cantidad = 0.ToString();
                    }
                    switch (Balance.GastoIngreso)
                    {
                        case "Servicios":
                            Balance.ImagenOrigen = "servicios";
                            break;
                        case "Ocio":
                            Balance.ImagenOrigen = "ocio";
                            break;
                        case "Provisiones":
                            Balance.ImagenOrigen = "provisiones";
                            break;
                        case "Impuestos":
                            Balance.ImagenOrigen = "impuestos";
                            break;
                        default:
                            Balance.ImagenOrigen = "Sin Imagen Disponible";
                            break;
                    }
                    ListaBalance.Add(Balance);

                }
            }

            CollectionBalance = new ObservableCollection<Balance>(ListaBalance.OrderByDescending(x => double.Parse(x.Dia)).ToList());

            BalanceTotal = ListaBalance.Sum(x => double.Parse(x.Cantidad)).ToString();
            if (double.Parse(BalanceTotal) < 0)
            {
                ColorBalance = Color.Red;
            }
            else if (double.Parse(BalanceTotal) > 0)
            {
                ColorBalance = Color.Green;
            }


        }

        public void Editar(Balance balance)
        {
            //Encuentro el balance a actualizar
            var balanceAntiguo = ListaBalance.Find(x => x.GastoIngreso == balance.GastoIngreso && x.BalanceId == balance.BalanceId);
            //Encuentro el gasto o ingreso  a actualizar en la base de datos y cambio uno a uno los valores
            if (balance.GastoIngreso == "Ingreso")
            {
                var IngresoAntiguo = dataService.Get<Ingresos>(true).Find(x => x.IngresoId.ToString() == balance.BalanceId);
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
            var confirmacion = await dialogService.ShowMessageConfirmacion("Mensaje", "¿Desea borrar este elemento?");
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
