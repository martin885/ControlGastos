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
    public partial class NotificationView : ContentPage
    {
        public NotificationView()
        {
            InitializeComponent();
            instance = this;
        }

        protected override void OnAppearing()
        {
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.Notification = new NotificationViewModel();
            InitializeComponent();
        }

        #region Singleton

        static NotificationView instance;

        public static NotificationView GetInstance()
        {
            if (instance == null)
            {
                return new NotificationView();
            }
            return instance;
        }
        #endregion

    }
}