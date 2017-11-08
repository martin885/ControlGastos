﻿using ControlGastos.Services;
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
    public class Balance
    {
        #region Servicios y propiedades
        NavigationService navigationService;
       public INavigation navigation { get; set; }
        #endregion
        
        #region Constructor
        public Balance()
        {
            navigationService = new NavigationService();
        }
        #endregion

        #region Propiedades para objeto Balance
        public string BalanceId { get; set; }

        public string Cantidad { get; set; }

        public string Fecha { get; set; }

        public string Dia { get; set; }

        public string Mes { get; set; }

        public string Anio { get; set; }

        public string Origen { get; set; }

        public string GastoIngreso { get; set; }

        public Color ColorGastoIngreso { get; set; }
        #endregion

        #region Commands
        public ICommand EditCommand
        {
            get
            {
                return new RelayCommand(Edit);
            }
        }


        private  void Edit()
        {
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.Edit = new EditViewModel(this);
        }
        #endregion

    }
}

