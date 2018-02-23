using ControlGastos.Behaviors;
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
    public class IngresosViewModel : INotifyPropertyChanged
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
        public string MesExcel { get; set; }
        public string Anio { get; set; }
        public int cont { get; set; }
        Ingresos Ingresos { get; set; }
        public BalanceViewModel Balance { get; set; }
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
                var filename = string.Format("Balance Mensual de Ingresos {0}-{1}", MesExcel, Anio);
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
            var ingresosView = IngresosView.GetInstance();
            var confirmacion = await dialogService.ShowMessageConfirmacion("Mensaje", "Desea exportar los ingresos a una planilla de cálculo");
            if (confirmacion)
            {
                DateSelected();
                if (ListaIngresos.Count == 0)
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

                    foreach (var elemento in ListaIngresos.Where(x => x.Mes.Equals(MesExcel) && x.Anio.Equals(Anio)).ToList())
                    {

                        // Writing Data.
                        //cont aumenta en 7 la posición de las filas en cada producto, las columnas dependen de los días elegidos

                        migrantRange["A1"].Text = "Fecha";
                        migrantRange["A1"].CellStyle.Font.Bold = true;

                        migrantRange["B1"].Text = "Ingreso";
                        migrantRange["B1"].CellStyle.Font.Bold = true;

                        migrantRange["C1"].Text = "Monto";
                        migrantRange["C1"].CellStyle.Font.Bold = true;


                        //Nueva celda
                        migrantRange.ResetRowColumn(cont + 2, 1);
                        migrantRange.Text = string.Format("{0}/{1}/{2}", elemento.Dia, elemento.Mes, elemento.Anio);


                        //migrantRange.CellStyle.Borders.LineStyle = ExcelLineStyle.Medium;

                        //Nueva celda
                        migrantRange.ResetRowColumn(cont + 2, 2);
                        migrantRange.Text = elemento.IngresoNombre;
                        //Nueva celda
                        migrantRange.ResetRowColumn(cont + 2, 3);

                        migrantRange.Number = double.Parse(elemento.IngresoCantidad);
                        if (double.Parse(elemento.IngresoCantidad) > 0)
                        {
                            worksheet[string.Format("C{0}", cont + 2)].CellStyle.Font.Color = ExcelKnownColors.Green;
                        }
                        else if (double.Parse(elemento.IngresoCantidad) < 0)
                        {
                            worksheet[string.Format("C{0}", cont + 2)].CellStyle.Font.Color = ExcelKnownColors.Red;
                        }


                        cont = cont + 1;

                    };

                    IRange range = worksheet.Range[string.Format("A{0}:B{0}", cont + 2)];
                    range.Merge();
                    range.Text = string.Format("Balance de Ingresos: ");
                    range.CellStyle.HorizontalAlignment = ExcelHAlign.HAlignCenter;
                    range.CellStyle.Font.Bold = true;
                    worksheet[string.Format("C{0}", cont + 2)].Number = double.Parse(SumaIngreso);
                    worksheet[string.Format("C{0}", cont + 2)].CellStyle.Font.Bold = true;
                    if (double.Parse(SumaIngreso) > 0)
                    {
                        worksheet[string.Format("C{0}", cont + 2)].CellStyle.ColorIndex = ExcelKnownColors.Green;
                    }
                    else if (double.Parse(SumaIngreso) < 0)
                    {
                        worksheet[string.Format("C{0}", cont + 2)].CellStyle.ColorIndex = ExcelKnownColors.Red;
                    }
                    worksheet.Range[string.Format("A1:C{0}", cont + 2)].BorderInside();
                    worksheet.Range[string.Format("A1:C{0}", cont + 2)].BorderAround();
                    worksheet.UsedRange.AutofitColumns();

                    //Save the workbook to stream in xlsx format. 
                    MemoryStream stream = new MemoryStream();
                    workbook.SaveAs(stream);

                    workbook.Close();

                    //Save the stream as a file in the device and invoke it for viewing
                    await DependencyService.Get<ISave>().SaveAndView(string.Format("Balance Mensual de Ingresos {0}-{1}", MesExcel, Anio) + ".xlsx", "application/msexcel", stream);

                    await dialogService.ShowMessage("Mensaje", string.Format("El balance se guardó como archivo de nombre '{0}' en la carpeta Balances", string.Format("Balance Mensual de Ingresos {0}-{1}", MesExcel, Anio) + ".xlsx"));
                }
            }
            ingresosView.excelUnTapped();
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

            if (MontoIngreso == "0" || string.IsNullOrEmpty(MontoIngreso) || string.IsNullOrWhiteSpace(MontoIngreso))
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
            CollectionIngresos = new ObservableCollection<Ingresos>(ListaIngresos.Where(x => x.Mes == Ingresos.Mes && x.Anio == Ingresos.Anio).OrderByDescending(x => double.Parse(x.Dia)).ToList());
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
            Mes = Date.ToString("MMMM", culture);
            MesExcel = Date.ToString("MMM", culture);
            Anio = Date.ToString("yyyy", culture);
            SumaIngreso = ListaIngresos.Where(x => x.Mes == Date.ToString("MMM", culture) &&
            x.Anio == Date.ToString("yyyy", culture)).ToList().Sum(x => double.Parse(x.IngresoCantidad)).ToString();
            CollectionIngresos = new ObservableCollection<Ingresos>(ListaIngresos.Where(x => x.Mes == Date.ToString("MMM", culture) && x.Anio == Date.ToString("yyyy", culture)).OrderByDescending(x => double.Parse(x.Dia)).ToList());
        }
        #endregion

        #region Constructor
        public IngresosViewModel()
        {
            dataService = new DataService();
            dialogService = new DialogService();
            Balance = MainViewModel.GetInstance().Balance;
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
            IsRefreshing = true;
            if (dataService.CheckTableIsEmpty<Ingresos>())
            {
                ListaIngresos = dataService.Get<Ingresos>(true);
                SumaIngreso = ListaIngresos.Where(x => x.Mes == Date.ToString("MMM", culture) &&
                x.Anio == Date.ToString("yyyy", culture)).ToList().Sum(x => double.Parse(x.IngresoCantidad)).ToString();
                CollectionIngresos = new ObservableCollection<Ingresos>(ListaIngresos.Where(x => x.Mes == Date.ToString("MMM", culture) &&
                x.Anio == Date.ToString("yyyy", culture)).OrderByDescending(x => double.Parse(x.Dia)).ToList());
            }
            else
            {
                SumaIngreso = "0";
            }
            IsRefreshing = false;
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
