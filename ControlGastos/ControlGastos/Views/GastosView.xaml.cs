
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

            InitializeComponent();
            instance = this;

        }
        protected override void OnAppearing()
        {
            //Actualización de la página
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