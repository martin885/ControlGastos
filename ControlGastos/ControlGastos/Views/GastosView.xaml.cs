﻿
using ControlGastos.ViewModels;
using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ControlGastos.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GastosView : ContentPage
    {
        
        public GastosView()
        {

            initialize();
            instance = this;

        }

        public void initialize()
        {

            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.Gastos = new GastosViewModel();
            InitializeComponent();
            
        }

        #region Singleton

        static GastosView instance;

        public static GastosView GetInstance()
        {
            if (instance == null)
            {
                return new GastosView();
            }
            return instance;
        }

        #endregion
        #region Eventos
        public void infoTapped(object sender, EventArgs e)
        {
            info.Opacity = 0.5;
        }

        private void correoTapped(object sender, EventArgs e)
        {
            correo.Opacity = 0.5;
        }
        private void excelTapped(object sender, EventArgs e)
        {
            excel.Opacity = 0.5;
        }
        public void excelUnTapped()
        {
            excel.Opacity = 1;
        }
        private void notificacionTapped(object sender, EventArgs e)
        {
            notificacion.Opacity = 0.5;
        }
        #endregion


        protected override void OnAppearing()
        {
            info.Opacity = 1;
            correo.Opacity = 1;
            excel.Opacity = 1;
            notificacion.Opacity = 1;
        }
      

    }
}