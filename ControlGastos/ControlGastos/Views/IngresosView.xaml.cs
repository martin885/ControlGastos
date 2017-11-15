﻿using ControlGastos.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ControlGastos.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IngresosView : ContentPage
    {
        public IngresosView()
        {
         
            InitializeComponent();
            instance = this;
        }
        protected override void OnAppearing()
        {
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.Ingresos = new IngresosViewModel();
            InitializeComponent();
        }

        #region Singleton

        static IngresosView instance;

        public static IngresosView GetInstance()
        {
            if (instance == null)
            {
                return new IngresosView();
            }
            return instance;
        }
        #endregion

    }
}