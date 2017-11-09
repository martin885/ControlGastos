using System.Threading.Tasks;
using Xamarin.Forms;

namespace ControlGastos.Services
{
    public  class DialogService
    {
        public async Task ShowMessage(string Title,string Message)
        {
            await Application.Current.MainPage.DisplayAlert(
                Title, 
                Message, 
                "Aceptar");
        }
        public async Task ShowMessageCompleto(string Title, string Message)
        {
            await Application.Current.MainPage.DisplayAlert(
                Title,
                Message,
                "Aceptar",
                "Cancelar");
        }
        public async Task<bool> ShowMessageConfirmacion(string Title, string Message)
        {
            return await Application.Current.MainPage.DisplayAlert(
                Title,
                Message,
                "Aceptar",
                "Cancelar");
        }
    }
}
