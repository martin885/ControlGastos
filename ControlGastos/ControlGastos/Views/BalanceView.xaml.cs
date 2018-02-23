using ControlGastos.Models;
using ControlGastos.ViewModels;
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
	public partial class BalanceView : ContentPage
	{
      
        public BalanceView ()
		{
			InitializeComponent ();
            instance = this;
        }


        protected override void OnAppearing()
        {
            
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.Balance = new BalanceViewModel();
            InitializeComponent();     
        }

        #region Singleton
        static BalanceView instance;

        public static BalanceView GetInstance()
        {
            if (instance == null)
            {
                return new BalanceView();
            }
            return instance;
        }
        #endregion




        #region Eventos
        private void infoTapped(object sender, EventArgs e)
        {
            info.Scale = 0.7;
            info.Source = "info2";
        }

        private void correoTapped(object sender, EventArgs e)
        {
            correo.Scale = 0.7;
            correo.Source = "correo2";
        }
        private void excelTapped(object sender, EventArgs e)
        {
            excel.Scale = 0.7;
            excel.VerticalOptions = LayoutOptions.Center;
            excel.Source = "excel2";
        }
        public void excelUnTapped()
        {
            excel.Scale = 1;
            excel.Source = "excel";
        }
        private void notificacionTapped(object sender, EventArgs e)
        {
            notificacion.Source = "notifiacion2";
        }
        #endregion
    }
}