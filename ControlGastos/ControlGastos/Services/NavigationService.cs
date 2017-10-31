using ControlGastos.Views;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ControlGastos.Services
{
    public  class NavigationService
    {

        public async Task Navigate( string PageName)
        {
            switch (PageName)
            {
                case "GastosView":
                    await Application.Current.MainPage.Navigation.PushAsync(new GastosView());
                    break;
                case "IngresosView":
                    await Application.Current.MainPage.Navigation.PushAsync(new IngresosView());
                    break;
                default:
                    break;
            }
            
        }

        public async Task Back()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}
