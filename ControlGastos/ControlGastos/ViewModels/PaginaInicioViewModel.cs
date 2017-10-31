using ControlGastos.Services;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
//Usar Nugget MvvmLightLibs para los commands sólo en el proyecto general, no en cada plataforma
namespace ControlGastos.ViewModels
{
    public class PaginaInicioViewModel
    {
        #region Services
        NavigationService navigationService;
        #endregion 

        #region Commands
        public ICommand GastosCommand
        {
            get
            {
                return new RelayCommand(Gastos);
            } 
        }
        private async void Gastos()
        {
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.Gastos = new GastosViewModel();
           await navigationService.Navigate("GastosView");
        }

        public ICommand IngresosCommand
        {
            get
            {
                return new RelayCommand(Ingresos);
            }
        }
        private async void Ingresos()
        {
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.Ingresos = new IngresosViewModel();
            await navigationService.Navigate("IngresosView");
        }

        #endregion
        #region Constructor
        public PaginaInicioViewModel()
        {
            navigationService = new NavigationService();
        }
        #endregion
    }
}
