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
                "Acceptar",
                "Cancelar");
        }
    }
}
