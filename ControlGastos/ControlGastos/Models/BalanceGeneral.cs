using ControlGastos.Services;
using ControlGastos.ViewModels;
using ControlGastos.Views;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ControlGastos.Models
{
   public class BalanceGeneral
    {
       
        
        #region Servicios y propiedades
        NavigationService navigationService;
        public INavigation navigation { get; set; }
        #endregion

        #region Constructor
        public BalanceGeneral()
        {
            navigationService = new NavigationService();
        }
        #endregion

        #region Propiedades para objeto BalanceGeneral
        public string BalanceId { get; set; }

        public string Cantidad { get; set; }

        public string CantidadIngreso { get; set; }

        public string CantidadGasto { get; set; }

        public string Fecha { get; set; }

        public string Dia { get; set; }

        public string Mes { get; set; }

        public string Anio { get; set; }

        public Color ColorGastoIngreso { get; set; }

        public string ImagenFecha { get; set; }

        public string ImagenMonto { get; set; }
        #endregion

        #region Commands
        //public ICommand EditCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(Edit);
        //    }
        //}
        //private async void Edit()
        //{
        //    var mainViewModel = MainViewModel.GetInstance();
        //    mainViewModel.Edit = new EditViewModel(this);
        //    var balanceView = BalanceView.GetInstance();
        //    await balanceView.Navigation.PushAsync(new EditView());
        //}
        //public ICommand DeleteCommand
        //{
        //    get
        //    {
        //        return new RelayCommand(Delete);
        //    }
        //}


        //private void Delete()
        //{
        //    var mainViewModel = MainViewModel.GetInstance();
        //    mainViewModel.Delete = new DeleteViewModel(this);
        //}
        #endregion
    }
}
