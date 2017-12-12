using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ControlGastos.Views;
using Xamarin.Forms;
using ControlGastos.ViewModels;
using ControlGastos.Services;
using Com.OneSignal;

namespace ControlGastos
{
    public partial class App : Application
    {
 

        public App()
        {
          
            InitializeComponent();
            
            //MainPage = new NavigationPage(new PaginaInicio());

            MainPage =new MenuTabbed();

            OneSignal.Current.StartInit("d18e950f-8242-437d-beb8-28fc657cf0a4").EndInit();

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
