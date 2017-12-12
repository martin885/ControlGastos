using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ControlGastos.ViewModels
{
    public class InfoViewModel:INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Commands
        public ICommand BrowseCommand
        {
            get
            {
                return new RelayCommand(Browse);
            }
        }

        private void Browse()
        {
            Device.OpenUri(new Uri("https://www.sumosistemas.com.ar"));
        }
        #endregion

        #region Constructor
        public InfoViewModel()
        {

        }
        #endregion

    }
}
