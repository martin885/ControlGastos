using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ControlGastos.Views;
using Xamarin.Forms;
using ControlGastos.ViewModels;
using ControlGastos.Services;
using Com.OneSignal;

using ControlGastos.Config;

namespace ControlGastos
{
    public partial class App : Application
    {
        public configuracion config;

        public App()
        {
          
            InitializeComponent();

            config = new configuracion();
            
            //MainPage = new NavigationPage(new PaginaInicio());

            MainPage =new NavigationPage( new MenuTabbed());

            OneSignal.Current.StartInit(config.IdOneSignal).EndInit();

        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
