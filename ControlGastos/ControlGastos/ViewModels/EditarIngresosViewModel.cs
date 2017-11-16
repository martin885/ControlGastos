using ControlGastos.Models;
using ControlGastos.Services;
using ControlGastos.Views;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ControlGastos.ViewModels
{
   public class EditarIngresosViewModel
    {
        #region Services
        NavigationService navigationService;
        DialogService dialogService;
        #endregion

        #region Propiedades y atributos
        private Ingresos ingresos;
        public string Fecha { get; set; }
        public string Dia { get; set; }
        public string Mes { get; set; }
        public string Anio { get; set; }
        public string GastoIngreso { get; set; }
        public string Origen { get; set; }
        public string Cantidad { get; set; }
        public string ImagenOrigen { get; set; }
        #endregion

        #region Command
        public ICommand GuardarCambioCommand
        {
            get
            {
                return new RelayCommand(GuardarCambio);
            }
        }

        private async void GuardarCambio()
        {
            try
            {

                if (Cantidad == "0" || string.IsNullOrEmpty(Cantidad) || string.IsNullOrWhiteSpace(Cantidad))
                {
                    await dialogService.ShowMessage("Error", "Debe asignar un valor mayor que cero");
                    return;
                }

            IFormatProvider culture = new CultureInfo("es-ES");

            Fecha = string.Format("{0}/{1}/{2}", Dia, Mes, Anio);



          
            
                Fecha = DateTime.Parse(Fecha, culture).ToString("dd/MMM/yyyy", culture);
                Dia = DateTime.Parse(Fecha, culture).ToString("dd", culture);
                Mes = DateTime.Parse(Fecha, culture).ToString("MMM", culture);
                Anio = DateTime.Parse(Fecha, culture).ToString("yyyy", culture);

            ingresos.Dia = Dia;
            ingresos.Mes = Mes;
            ingresos.Anio = Anio;
            ingresos.IngresoNombre = string.Format("{0}{1}", Origen.Substring(0, 1).ToUpper(), Origen.Substring(1));

            if (Cantidad.Contains("-"))
            {
                ingresos.IngresoCantidad = Cantidad.Replace("-", "");
            }
            else
            {
                ingresos.IngresoCantidad = Cantidad;
            }
            var ingresosViewModel = IngresosViewModel.GetInstance();
            ingresosViewModel.Editar(ingresos);
            var editarIngresosView = EditarIngresosView.GetInstance();
            await editarIngresosView.Navigation.PopAsync();
            }
            catch
            {
                await dialogService.ShowMessage("Error", "El formato elegido es incorrecto");
                return;
            }
        }
        #endregion

        #region Constructor
        public EditarIngresosViewModel(Ingresos ingresos)
        {

            navigationService = new NavigationService();
            dialogService = new DialogService();

            this.ingresos = ingresos;
            //Carga los valores en la página de edición
            Dia = ingresos.Dia;
            Mes = ingresos.Mes;
            Anio = ingresos.Anio;
            if (string.IsNullOrEmpty(ingresos.IngresoNombre))
            {
                Origen = "Sin origen";
            }
            else
            {
                Origen = ingresos.IngresoNombre;
            }

            Cantidad = ingresos.IngresoCantidad;
            ImagenOrigen = "income";

            #endregion
        }
    }
}