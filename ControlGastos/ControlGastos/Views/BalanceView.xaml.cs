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
		}


        protected override void OnAppearing()
        {
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.Balance = new BalanceViewModel();
            InitializeComponent();
        }

        private async void MenuItem_Clicked(object sender, EventArgs e)
        {
            //Navegación hacia la vista de edición, la lógica va con el command en el modelo de Balance
           await Navigation.PushAsync(new EditView());

        }
    }
}