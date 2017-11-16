using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ControlGastos.Models;
using Xamarin.Forms;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using ControlGastos.Services;
using System.Globalization;
using ControlGastos.Views;

namespace ControlGastos.ViewModels
{
    public class EditarGastosViewModel
    {
        #region Services
        NavigationService navigationService;
        DialogService dialogService;
        #endregion

        #region Propiedades y atributos
        private Gastos gastos;
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
            

            gastos.Dia = Dia;
            gastos.Mes = Mes;
            gastos.Anio = Anio;
            gastos.GastoNombre = string.Format("{0}{1}", Origen.Substring(0, 1).ToUpper(), Origen.Substring(1));

            if (Cantidad.Contains("-"))
            {
                gastos.GastosCantidad = Cantidad.Replace("-", "");
            }
            else
            {
                gastos.GastosCantidad = Cantidad;
            }
            // Instanciar la GastosViewModel para usar el método Editar
            var gastosViewModel = GastosViewModel.GetInstance();
            gastosViewModel.Editar(gastos);
            // Instanciar la pagina de editar gastos para usar la propiedad de navegación
            var editarGastos = EditarGastos.GetInstance();
            await editarGastos.Navigation.PopAsync();
            //await navigationService.Back();
        }
            catch
            {
                await dialogService.ShowMessage("Error", "El formato elegido es incorrecto");
                return;
            }
        }
        #endregion

        #region Constructor
        public EditarGastosViewModel(Gastos gastos)
        {

            navigationService = new NavigationService();
            dialogService = new DialogService();

            this.gastos = gastos;

            Dia = gastos.Dia;
            Mes = gastos.Mes;
            Anio = gastos.Anio;
            if (string.IsNullOrEmpty(gastos.GastoNombre))
            {
                Origen = "Sin origen";
            }
            else
            {
                Origen = gastos.GastoNombre;
            }

            if (gastos.GastosCantidad.Contains("-"))
            {
                Cantidad = gastos.GastosCantidad.Replace("-", "");
            }
            else
            {
                Cantidad = gastos.GastosCantidad;
            }

                switch (gastos.Categoria)
                {
                    case "Servicios":
                        ImagenOrigen = "servicios";
                        break;
                    case "Ocio":
                        ImagenOrigen = "ocio";
                        break;
                    case "Provisiones":
                        ImagenOrigen = "provisiones";
                        break;
                case "Impuestos":
                    ImagenOrigen = "impuestos";
                    break;
                default:
                        ImagenOrigen = "Sin Imagen Disponible";
                        break;
                }
            
        }
        #endregion

    }
}

