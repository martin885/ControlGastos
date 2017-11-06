using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ControlGastos.Views;
using Xamarin.Forms;
using ControlGastos.ViewModels;
using ControlGastos.Services;

namespace ControlGastos
{
    public partial class App : Application
    {
 

        public App()
        {
          
            InitializeComponent();
            
            //MainPage = new NavigationPage(new PaginaInicio());

            MainPage = new MenuTabbed();

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
