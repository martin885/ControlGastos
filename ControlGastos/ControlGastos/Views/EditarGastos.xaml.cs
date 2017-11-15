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
	public partial class EditarGastos : ContentPage
	{
		public EditarGastos ()
		{
			InitializeComponent ();
            instance = this;
        }

        #region Singleton

        static EditarGastos instance;

        public static EditarGastos GetInstance()
        {
            if (instance == null)
            {
                return new EditarGastos();
            }
            return instance;
        }
        #endregion
    }
}