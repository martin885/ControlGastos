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
    public class BalanceGeneralViewModel : INotifyPropertyChanged
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
        public List<BalanceGeneral> ListaBalanceGeneral { get; set; }
        public string SelectedItemAño { get; set; }
        public string SelectedItemMes { get; set; }
        public string DefaultAño { get; set; }
        public string DefaultMes { get; set; }
        public bool ExisteObjeto { get; set; }
        BalanceGeneral BalanceGeneral { get; set; }
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

        ObservableCollection<BalanceGeneral> _collectionBalance;
        public ObservableCollection<BalanceGeneral> CollectionBalance
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
                var filename = string.Format("Balance General");
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
            var balanceGeneralView = BalanceGeneralView.GetInstance();

            var confirmacion = await dialogService.ShowMessageConfirmacion("Mensaje", "¿Desea exportar el balance a una planilla de cálculo?");
            if (confirmacion)
            {
                Cargas();
                if (ListaBalanceGeneral.Count == 0)
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

                    foreach (var elemento in ListaBalanceGeneral)
                    {

                        // Writing Data.
                        //cont aumenta en 7 la posición de las filas en cada producto, las columnas dependen de los días elegidos

                        migrantRange["A1"].Text = "Fecha";
                        migrantRange["A1"].CellStyle.Font.Bold = true;

                        migrantRange["B1"].Text = "Gastos";
                        migrantRange["B1"].CellStyle.Font.Bold = true;

                        migrantRange["C1"].Text = "Ingresos";
                        migrantRange["C1"].CellStyle.Font.Bold = true;

                        migrantRange["D1"].Text = "Monto";
                        migrantRange["D1"].CellStyle.Font.Bold = true;

                        //Nueva celda
                        migrantRange.ResetRowColumn(cont + 2, 1);
                        migrantRange.Text = string.Format("{0}/{1}/{2}", elemento.Dia, elemento.Mes, elemento.Anio);


                        //migrantRange.CellStyle.Borders.LineStyle = ExcelLineStyle.Medium;

                        //Nueva celda
                        migrantRange.ResetRowColumn(cont + 2, 2);
                        migrantRange.Text = elemento.CantidadGasto;
                        //Nueva celda
                        migrantRange.ResetRowColumn(cont + 2, 3);
                        migrantRange.Text = elemento.CantidadIngreso;
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
                    range.Text = string.Format("Balance General: ");
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
                    await DependencyService.Get<ISave>().SaveAndView(string.Format("Balance General") + ".xlsx", "application/msexcel", stream);

                    await dialogService.ShowMessage("Mensaje", string.Format("El balance se guardó como archivo de nombre '{0}' en la carpeta Balances", string.Format("Balance General") + ".xlsx"));
                }
            }
            balanceGeneralView.excelUnTapped();
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
        public BalanceGeneralViewModel()
        {

            instance = this;
            dataService = new DataService();
            dialogService = new DialogService();

            Cargas();

        }
        #endregion

        #region Singleton

        static BalanceGeneralViewModel instance;

        public static BalanceGeneralViewModel GetInstance()
        {
            if (instance == null)
            {
                return new BalanceGeneralViewModel();
            }
            return instance;
        }
        #endregion

        #region Métodos
        private void Cargas()
        {
            culture = new CultureInfo("es-ES");
            ListaBalanceGeneral = new List<BalanceGeneral>();
            IsRefreshing = true;

            CargarLaObservableCollection();

            IsRefreshing = false;
        }

        private void CargarLaObservableCollection()
        {
            ListaBalanceGeneral.Clear();

            //Agregado de objetos balanceGeneral a las observable collection en caso de que se encuentren el mes y el año pertenecientes a la fecha del día. En función de los ingresos 

            foreach (var ListaIngresoPorAño in dataService.Get<Ingresos>(true).GroupBy(x => x.Anio).ToList())
            {
                foreach (var ListaIngresoPorMes in ListaIngresoPorAño.GroupBy(x => x.Mes).ToList())
                {
                    ExisteObjeto = false;
                    foreach (var CorroborarSiExisteObjeto in ListaBalanceGeneral)
                    {
                        if (CorroborarSiExisteObjeto.Mes.Equals(ListaIngresoPorMes.FirstOrDefault().Mes)
                            && CorroborarSiExisteObjeto.Anio.Equals(ListaIngresoPorMes.FirstOrDefault().Anio))
                        {
                            ExisteObjeto = true;
                        }
                    }
                    if (ExisteObjeto)
                    {
                        var TotalDelMes = ListaIngresoPorMes.Sum(x => int.Parse(x.IngresoCantidad)).ToString();

                        var BalanceGeneralUpdate = ListaBalanceGeneral.Find(x => x.Mes.Equals(ListaIngresoPorMes.FirstOrDefault().Mes)
                         && x.Anio.Equals(ListaIngresoPorMes.FirstOrDefault().Anio));

                        BalanceGeneralUpdate.CantidadIngreso = TotalDelMes;

                    }
                    else
                    {
                        var TotalDelMes = ListaIngresoPorMes.Sum(x => int.Parse(x.IngresoCantidad)).ToString();

                        BalanceGeneral = new BalanceGeneral
                        {
                            Mes = ListaIngresoPorMes.FirstOrDefault().Mes,
                            Anio = ListaIngresoPorMes.FirstOrDefault().Anio,
                            Fecha = string.Format("{0}/{1}", ListaIngresoPorMes.FirstOrDefault().Mes, ListaIngresoPorMes.FirstOrDefault().Anio),
                            ImagenFecha = "date",
                            CantidadIngreso = TotalDelMes,
                            CantidadGasto = "0",
                            ImagenMonto = "money"
                        };
                        if (!double.TryParse(BalanceGeneral.CantidadIngreso, out double result))
                        {
                            BalanceGeneral.CantidadIngreso = 0.ToString();
                        }

                        ListaBalanceGeneral.Add(BalanceGeneral);
                    }
                }
            }
            //Agregado de objetos balance a las observable collection en caso de que se encuentren el mes y el año pertenecientes a la fecha del día. En función de los gastos 

            foreach (var ListaGastoPorAño in dataService.Get<Gastos>(true).GroupBy(x => x.Anio).ToList())
            {
                foreach (var ListaGastoPorMes in ListaGastoPorAño.GroupBy(x => x.Mes).ToList())
                {
                    ExisteObjeto = false;

                    foreach (var CorroborarSiExisteObjeto in ListaBalanceGeneral)
                    {
                        if (CorroborarSiExisteObjeto.Mes.Equals(ListaGastoPorMes.FirstOrDefault().Mes)
                            && CorroborarSiExisteObjeto.Anio.Equals(ListaGastoPorMes.FirstOrDefault().Anio))
                        {
                            ExisteObjeto = true;
                        }
                    }

                    if (ExisteObjeto)
                    {
                        var TotalDelMes = ListaGastoPorMes.Sum(x => int.Parse(x.GastosCantidad)).ToString();

                        var BalanceGeneralUpdate = ListaBalanceGeneral.Find(x => x.Mes.Equals(ListaGastoPorMes.FirstOrDefault().Mes)
                           && x.Anio.Equals(ListaGastoPorMes.FirstOrDefault().Anio));

                        BalanceGeneralUpdate.CantidadGasto = TotalDelMes;
                    }
                    else
                    {
                        var TotalDelMes = ListaGastoPorMes.Sum(x => int.Parse(x.GastosCantidad)).ToString();

                        BalanceGeneral = new BalanceGeneral
                        {

                            Mes = ListaGastoPorMes.FirstOrDefault().Mes,
                            Anio = ListaGastoPorMes.FirstOrDefault().Anio,
                            Fecha = string.Format("{0}/{1}", ListaGastoPorMes.FirstOrDefault().Mes, ListaGastoPorMes.FirstOrDefault().Anio),
                            ImagenFecha = "date",
                            CantidadGasto = string.Format("-{0}", TotalDelMes),
                            CantidadIngreso = "0",
                            ImagenMonto = "money"
                        };
                        if (BalanceGeneral.CantidadGasto.Contains("--"))
                        {
                            BalanceGeneral.CantidadGasto = BalanceGeneral.CantidadGasto.Replace("--", "-");
                        }
                        if (!double.TryParse(BalanceGeneral.CantidadGasto, out double result))
                        {
                            BalanceGeneral.Cantidad = 0.ToString();
                        }

                        ListaBalanceGeneral.Add(BalanceGeneral);
                    }


                }
            }


            foreach (var balanceGeneral in ListaBalanceGeneral)
            {
                if (balanceGeneral.CantidadGasto.Equals(null))
                {
                    balanceGeneral.CantidadGasto = "0";
                }
                if (balanceGeneral.CantidadIngreso.Equals(null))
                {
                    balanceGeneral.CantidadIngreso = "0";
                }
                balanceGeneral.Cantidad = string.Format("{0}", (int.Parse(balanceGeneral.CantidadGasto) + int.Parse(balanceGeneral.CantidadIngreso)));
                if (double.Parse(balanceGeneral.Cantidad) < 0)
                {
                    balanceGeneral.ColorGastoIngreso = Color.Red;
                }
                else if(double.Parse(balanceGeneral.Cantidad) > 0)
                {
                    balanceGeneral.ColorGastoIngreso = Color.Green;
                }
            }

            CollectionBalance = new ObservableCollection<BalanceGeneral>(ListaBalanceGeneral.OrderByDescending(x => double.Parse(x.Anio)).ToList());

            BalanceTotal = ListaBalanceGeneral.Sum(x => double.Parse(x.Cantidad)).ToString();
            if (double.Parse(BalanceTotal) < 0)
            {
                ColorBalance = Color.Red;
            }
            else if (double.Parse(BalanceTotal) > 0)
            {
                ColorBalance = Color.Green;
            }


        }

        public async void Delete(BalanceGeneral balanceGeneral)
        {
            var confirmacion = await dialogService.ShowMessageConfirmacion("Mensaje", "¿Desea borrar este elemento?");

            if (confirmacion)
            {
                var balanceGeneralBorrar = ListaBalanceGeneral.Find(x => x.Mes.Equals(balanceGeneral.Mes) && x.Anio.Equals(balanceGeneral.Anio));

                foreach (var IngresoParaBorrar in dataService.Get<Ingresos>(true).Where(x => x.Anio.Equals(balanceGeneral.Anio) && x.Mes.Equals(balanceGeneral.Mes)).ToList())
                {
                    dataService.Delete(IngresoParaBorrar);
                }
                foreach (var GastoParaBorrar in dataService.Get<Gastos>(true).Where(x => x.Anio.Equals(balanceGeneral.Anio) && x.Mes.Equals(balanceGeneral.Mes)).ToList())
                {
                    dataService.Delete(GastoParaBorrar);
                }
                ListaBalanceGeneral.Remove(balanceGeneralBorrar);

                CargarLaObservableCollection();
            }
            else
            {
                return;
            }


        }
        #endregion
    }
}
