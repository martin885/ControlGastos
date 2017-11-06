
using ControlGastos.ViewModels;
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
        }
        protected override void OnAppearing()
        {
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.Gastos = new GastosViewModel();
            InitializeComponent();
        }

    }
}