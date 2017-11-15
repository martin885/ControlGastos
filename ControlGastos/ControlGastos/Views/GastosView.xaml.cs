
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
    }
}