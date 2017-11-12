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
            IFormatProvider culture = new CultureInfo("es-ES");

            Fecha = string.Format("{0}/{1}/{2}", Dia, Mes, Anio);
           
            if (!DateTime.TryParse(Fecha,out DateTime Result))
            {
               await dialogService.ShowMessage("Error", "El formato elegido es incorrecto");
                return;
            }


            Fecha = DateTime.Parse(Fecha, culture).ToString("dd/MMM/yyyy",culture);
            Dia = DateTime.Parse(Fecha).ToString("dd",culture);
            Mes = DateTime.Parse(Fecha).ToString("MMM",culture);
            Anio = DateTime.Parse(Fecha).ToString("yyyy",culture);
            balance.Dia = Dia;
            balance.Mes = Mes;
            balance.Anio = Anio;
            balance.GastoIngreso = GastoIngreso;
            balance.Origen = Origen;

            if (Cantidad.Contains("-"))
            {
                balance.Cantidad = Cantidad.Replace("-","");
            }
            else { 
            balance.Cantidad = Cantidad;
            }
            var balanceViewModel = BalanceViewModel.GetInstance();
            balanceViewModel.Editar(balance);
            var editView = EditView.GetInstance();
           await editView.Navigation.PopAsync();
           //await navigationService.Back();
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
            GastoIngreso = balance.GastoIngreso;
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
        }
        #endregion

    }
}
