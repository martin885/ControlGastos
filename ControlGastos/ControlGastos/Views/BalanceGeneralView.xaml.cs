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
	public partial class BalanceGeneralView : ContentPage
	{
		public BalanceGeneralView ()
		{


            initialize();
            instance = this;
        }

        public void initialize()
        {
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.BalanceGeneral = new BalanceGeneralViewModel();
            InitializeComponent();            
        }

        

        #region Singleton
        static BalanceGeneralView instance;

        public static BalanceGeneralView GetInstance()
        {
            if (instance == null)
            {
                return new BalanceGeneralView();
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