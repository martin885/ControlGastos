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
    }
}