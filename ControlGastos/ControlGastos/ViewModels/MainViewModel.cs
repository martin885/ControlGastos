﻿using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ControlGastos.ViewModels
{
    public class MainViewModel
    {
        #region Propiedades
        public PaginaInicioViewModel PaginaInicio { get; set; }
        public GastosViewModel Gastos { get; set; }
        public IngresosViewModel Ingresos { get; set; }
        public BalanceViewModel Balance { get; set; }
        #endregion


        #region Constructor
        public MainViewModel()
        {
            instance = this;
            //PaginaInicio = new PaginaInicioViewModel();
            Balance = new BalanceViewModel();
            Gastos = new GastosViewModel();
            Ingresos = new IngresosViewModel();
        }
        #endregion

        #region Singleton

       static MainViewModel instance;

      public static MainViewModel GetInstance()
        {
            if (instance == null)
            {
               return new MainViewModel();
            }
            return instance;
        }
        #endregion

    }
}
