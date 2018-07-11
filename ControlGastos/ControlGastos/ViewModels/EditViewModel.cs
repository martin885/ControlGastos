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
    public class EditViewModel
    {
        #region Services
        NavigationService navigationService;
        DialogService dialogService;
        #endregion

        #region Propiedades y atributos
        private Balance balance;
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


            balance.Dia = Dia;
            balance.Mes = Mes;
            balance.Anio = Anio;
            balance.Origen = string.Format("{0}{1}", Origen.Substring(0, 1).ToUpper(), Origen.Substring(1));
                if (balance.GastoIngreso.Equals("Ingreso"))
                {
                    if (Cantidad.Contains("-"))
                    {
                        balance.Cantidad = Cantidad.Replace("-","");
                    }
                    else
                    {
                        balance.Cantidad = string.Format("{0}", Cantidad);
                    }
                }
                else
                {
                    if (!Cantidad.Contains("-"))
                    {
                        balance.Cantidad = string.Format("-{0}", Cantidad);
                    }
                    else
                    {
                        balance.Cantidad = string.Format("{0}", Cantidad);
                    }
                }

            var balanceViewModel = BalanceViewModel.GetInstance();
            balanceViewModel.Editar(balance);
            var editView = EditView.GetInstance();
           await editView.Navigation.PopAsync();
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
        public EditViewModel(Balance balance)
        {
           
            navigationService = new NavigationService();
            dialogService = new DialogService();

            this.balance = balance;
  
            Dia = balance.Dia;
            Mes = balance.Mes;
            Anio = balance.Anio;
            if (string.IsNullOrEmpty(balance.Origen))
            {
                Origen = "Sin origen";
            }
            else
            {
                Origen = balance.Origen;
            }

            if (balance.Cantidad.Contains("-")) { 
            Cantidad = balance.Cantidad.Replace("-","");
            }
            else
            {
                Cantidad = balance.Cantidad;
            }
            if (balance.GastoIngreso.Contains("Ingreso"))
            {
                ImagenOrigen = "income";
            }
            else { 
            switch (balance.GastoIngreso)
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
        }
        #endregion

    }
}
