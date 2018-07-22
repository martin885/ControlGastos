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
    public class GastosViewModel : INotifyPropertyChanged
    {

        #region Eventos
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Services 
        DataService dataService;
        DialogService dialogService;
        InstanciarPaginasService instanciarPaginasService;
        #endregion

        #region Propiedades y Atributos
        IFormatProvider culture;
        public DateTime Date { get; set; }
        public int cont { get; set; }
        public string MesExcel { get; set; }
        public string Anio { get; set; }
        public string SelectedItem { get; set; }
        public BalanceViewModel Balance { get; set; }
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

        string _origenGasto;
        public string OrigenGasto
        {
            get
            {
                return _origenGasto;
            }
            set
            {
                if (_origenGasto != value)
                {
                    _origenGasto = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(OrigenGasto)));
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
                var filename = string.Format("Balance Mensual de Gastos {0}-{1}", MesExcel, Anio);
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
            var gastosView = GastosView.GetInstance();
            var confirmacion = await dialogService.ShowMessageConfirmacion("Mensaje", "¿Desea exportar los gastos a una planilla de cálculo?");
            if (confirmacion)
            {
                try
                {
                    DateSelected();
                    if (ListaGastos.Count == 0)
                    {
                        await dialogService.ShowMessage("Error", "Se deben agregar elementos al balance");
                        gastosView.excelUnTapped();
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

                        foreach (var elemento in ListaGastos.Where(x => x.Mes.Equals(MesExcel) && x.Anio.Equals(Anio)).ToList())
                        {

                            // Writing Data.
                            //cont aumenta en 7 la posición de las filas en cada producto, las columnas dependen de los días elegidos

                            migrantRange["A1"].Text = "Fecha";
                            migrantRange["A1"].CellStyle.Font.Bold = true;

                            migrantRange["B1"].Text = "Gasto";
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
                            migrantRange.Text = elemento.GastoNombre;
                            //Nueva celda
                            migrantRange.ResetRowColumn(cont + 2, 3);
                            migrantRange.Text = elemento.Categoria;
                            //Nueva celda
                            migrantRange.ResetRowColumn(cont + 2, 4);

                            migrantRange.Number = double.Parse(elemento.GastosCantidad);
                            if (double.Parse(elemento.GastosCantidad) > 0)
                            {
                                worksheet[string.Format("D{0}", cont + 2)].CellStyle.Font.Color = ExcelKnownColors.Green;
                            }
                            else if (double.Parse(elemento.GastosCantidad) < 0)
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
                        worksheet[string.Format("D{0}", cont + 2)].Number = double.Parse(SumaGasto);
                        worksheet[string.Format("D{0}", cont + 2)].CellStyle.Font.Bold = true;
                        if (double.Parse(SumaGasto) > 0)
                        {
                            worksheet[string.Format("D{0}", cont + 2)].CellStyle.ColorIndex = ExcelKnownColors.Green;
                        }
                        else if (double.Parse(SumaGasto) < 0)
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
                         DependencyService.Get<ISave>().SaveAndView(string.Format("Balance Mensual de Gastos {0}-{1}", MesExcel, Anio) + ".xlsx", "application/msexcel", stream);

                        await dialogService.ShowMessage("Mensaje", string.Format("El balance se guardó como archivo de nombre '{0}' en la carpeta Balances", string.Format("Balance Mensual de Gastos {0}-{1}", MesExcel, Anio) + ".xlsx"));
                    }
                }
               catch(Exception e)
                {
                   await dialogService.ShowMessage("Error", "No se pudo exportar a hoja de cálculo. Intenta habilitando los permisos en ajustes.");
                }
            }
            gastosView.excelUnTapped();
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
            
            if (MontoGasto == "0" || string.IsNullOrEmpty(MontoGasto) || string.IsNullOrWhiteSpace(MontoGasto))
            {
                await dialogService.ShowMessage("Error", "Debe asignar un valor mayor que cero");
                return;
            }

            if (SelectedItem == null)
            {
                await dialogService.ShowMessage("Error",
                    "Se debe seleccionar una categoría");
                return;
            }
            Gastos = new Gastos();
            Gastos.Anio = Date.ToString("yyyy", culture);
            Gastos.Mes = Date.ToString("MMM", culture);
            Gastos.Dia = Date.ToString("dd", culture);
            Gastos.ImagenFecha = "date";
            if (string.IsNullOrEmpty(OrigenGasto))
            {
                OrigenGasto = "Sin Origen";
            }

            Gastos.Categoria = SelectedItem;
            Gastos.GastoNombre = string.Format("{0}{1}", OrigenGasto.Substring(0, 1).ToUpper(), OrigenGasto.Substring(1));
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
                case "Impuestos":
                    Gastos.ImagenOrigen = "impuestos";
                    break;
                default:
                    Gastos.ImagenOrigen = "Sin Imagen Disponible";
                    break;
            }
            if (MontoGasto == null)
            {
                MontoGasto = 0.ToString();
            }
            if (!double.TryParse(MontoGasto, out double result))
            {
                await dialogService.ShowMessage("Error", "El contenido del monto debe ser un número");
                MontoGasto = null;
                return;
            }
            if (!MontoGasto.Contains("-"))
            {
                Gastos.GastosCantidad = string.Format("-{0}", MontoGasto);
            }
            else
            {
                Gastos.GastosCantidad = string.Format("{0}", MontoGasto);
            }
            Gastos.ImagenMonto = "money";
            ListaGastos.Add(Gastos);
            //Realizar la sumatoria con los ingresos pertenecientes al mes y año elegido
            SumaGasto = ListaGastos.Where(x => x.Mes == Gastos.Mes && x.Anio == Gastos.Anio).ToList().Sum(x => double.Parse(x.GastosCantidad)).ToString();
            SumaGastoCategoria = ListaGastos.Where(x => x.Mes == Gastos.Mes && x.Anio == Gastos.Anio && x.Categoria == SelectedItem).ToList().Sum(x => double.Parse(x.GastosCantidad)).ToString();
            MontoGasto = null;
            OrigenGasto = null;
            dataService.Save(ListaGastos, true);
            CollectionGastos = new ObservableCollection<Gastos>(ListaGastos.Where(x => x.Mes == Gastos.Mes && x.Anio == Gastos.Anio && x.Categoria == SelectedItem).OrderByDescending(x => double.Parse(x.Dia)).ToList());
            instanciarPaginasService.Instanciar();
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
            SumaGasto = ListaGastos.Where(x => x.Mes == Date.ToString("MMM", culture) &&
            x.Anio == Date.ToString("yyyy", culture)).ToList().Sum(x => double.Parse(x.GastosCantidad)).ToString();
            SumaGastoCategoria = ListaGastos.Where(x => x.Mes == Date.ToString("MMM", culture) &&
            x.Anio == Date.ToString("yyyy", culture) &&
            x.Categoria == SelectedItem).ToList().Sum(x => int.Parse(x.GastosCantidad)).ToString();
            if (string.IsNullOrEmpty(Categoria) || Categoria.Equals("SIN SELECCIONAR"))
            {
                CollectionGastos = new ObservableCollection<Gastos>(ListaGastos.Where(x => x.Mes == Date.ToString("MMM", culture) &&
x.Anio == Date.ToString("yyyy", culture)).OrderByDescending(x => double.Parse(x.Dia)).ToList());
            }
            else
            {
                CollectionGastos = new ObservableCollection<Gastos>(ListaGastos.Where(x => x.Mes == Date.ToString("MMM", culture) &&
x.Anio == Date.ToString("yyyy", culture) && x.Categoria.Equals(Categoria)).OrderByDescending(x => double.Parse(x.Dia)).ToList());
            }

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
            SumaGastoCategoria = ListaGastos.Where(x => x.Mes == Date.ToString("MMM", culture) &&
            x.Anio == Date.ToString("yyyy", culture) &&
            x.Categoria == SelectedItem).ToList().Sum(x => int.Parse(x.GastosCantidad)).ToString();
            CollectionGastos = new ObservableCollection<Gastos>(ListaGastos.Where(x => x.Mes == Date.ToString("MMM", culture) &&
            x.Anio == Date.ToString("yyyy", culture) &&
            x.Categoria == SelectedItem).OrderByDescending(x => double.Parse(x.Dia)).ToList());
        }
        #endregion

        #region Constructor
        public GastosViewModel()
        {
            culture = new CultureInfo("es-ES");
            dataService = new DataService();
            dialogService = new DialogService();
            instanciarPaginasService = new InstanciarPaginasService();

            Balance = MainViewModel.GetInstance().Balance;
            instance = this;
            Cargas();

        }
        #endregion

        #region Singleton

        static GastosViewModel instance;

        public static GastosViewModel GetInstance()
        {
            if (instance == null)
            {
                return new GastosViewModel();
            }
            return instance;
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
            PickerCategorias.Add("Impuestos");
            Mes = DateTime.Now.ToString("MMMM", culture);
            Date = DateTime.Now;
            ListaGastos = new List<Gastos>();
            IsRefreshing = true;
            if (dataService.CheckTableIsEmpty<Gastos>())
            {
                //Busco en la base de datos los gastos guardados 
                try
                {
                    ListaGastos = dataService.Get<Gastos>(true);
                    SumaGastoCategoria = ListaGastos.Where(x => x.Mes == Date.ToString("MMM", culture) &&
                    x.Anio == Date.ToString("yyyy", culture) &&
                    x.Categoria == SelectedItem).ToList().Sum(x => double.Parse(x.GastosCantidad)).ToString();
                    SumaGasto = ListaGastos.Where(x => x.Mes == Date.ToString("MMM", culture) &&
                    x.Anio == Date.ToString("yyyy", culture)).ToList().Sum(x => double.Parse(x.GastosCantidad)).ToString();
                    CollectionGastos = new ObservableCollection<Gastos>(ListaGastos.Where(x => x.Mes == Date.ToString("MMM", culture) &&
                    x.Anio == Date.ToString("yyyy", culture)).OrderByDescending(x => double.Parse(x.Dia)).ToList());

                }
                catch
                {

                }
            }
            else
            {

                SumaGasto = "0";

            }
            IsRefreshing = false;
        }

        public void Editar(Gastos gastos)
        {
            //Busco el gasto antiguo en la lista actual y después se lo reemplaza por el nuevo editado
            var gastoAntiguo = ListaGastos.Find(x => x.GastosId == gastos.GastosId);
            gastoAntiguo = gastos;
            dataService.Update(gastoAntiguo, true);
            SumaGastoCategoria = ListaGastos.Where(x => x.Mes == Date.ToString("MMM", culture) &&
          x.Anio == Date.ToString("yyyy", culture) &&
          x.Categoria == SelectedItem).ToList().Sum(x => double.Parse(x.GastosCantidad)).ToString();
            SumaGasto = ListaGastos.Where(x => x.Mes == Date.ToString("MMM", culture) &&
            x.Anio == Date.ToString("yyyy", culture)).ToList().Sum(x => double.Parse(x.GastosCantidad)).ToString();
            CollectionGastos = new ObservableCollection<Gastos>(ListaGastos.OrderByDescending(x => double.Parse(x.Dia)).ToList());
            instanciarPaginasService.Instanciar();
        }

        public async void Delete(Gastos gastos)
        {
            var confirmacion = await dialogService.ShowMessageConfirmacion("Mensaje", "¿Desea borrar este elemento?");

            if (confirmacion)
            {

                var GastoAntiguo = ListaGastos.Find(x => x.GastosId.Equals(gastos.GastosId));
                dataService.Delete(GastoAntiguo);
                ListaGastos.Remove(GastoAntiguo);
                SumaGastoCategoria = ListaGastos.Where(x => x.Mes == Date.ToString("MMM", culture) &&
               x.Anio == Date.ToString("yyyy", culture) &&
               x.Categoria == SelectedItem).ToList().Sum(x => double.Parse(x.GastosCantidad)).ToString();
                SumaGasto = ListaGastos.Where(x => x.Mes == Date.ToString("MMM", culture) &&
                x.Anio == Date.ToString("yyyy", culture)).ToList().Sum(x => double.Parse(x.GastosCantidad)).ToString();
                CollectionGastos = new ObservableCollection<Gastos>(ListaGastos.OrderByDescending(x => double.Parse(x.Dia)).ToList());
                instanciarPaginasService.Instanciar();
            }
            else
            {
                return;
            }
        }
        #endregion
    }



}

