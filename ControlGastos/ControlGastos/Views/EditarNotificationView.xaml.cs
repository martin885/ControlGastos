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
    public partial class EditarNotificationView : ContentPage
    {
        public EditarNotificationView()
        {
            InitializeComponent();
            instance = this;
        }

        #region Singleton

        static EditarNotificationView instance;

        public static EditarNotificationView GetInstance()
        {
            if (instance == null)
            {
                return new EditarNotificationView();
            }
            return instance;
        }
        #endregion

    }
}