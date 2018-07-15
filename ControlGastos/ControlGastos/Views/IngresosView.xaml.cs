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
    public partial class IngresosView : ContentPage
    {
        public IngresosView()
        {

            initialize();
            instance = this;
        }

        public void initialize()
        {

            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.Ingresos=new IngresosViewModel();
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

        public void botones()
        {
            info.Opacity = 1;
            correo.Opacity = 1;
            excel.Opacity = 1;
            notificacion.Opacity = 1;
        }

    }
}