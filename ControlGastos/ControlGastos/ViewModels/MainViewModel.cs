using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ControlGastos.ViewModels
{
    public class MainViewModel
    {
        #region Propiedades
        public PaginaInicioViewModel PaginaInicio { get; set; }
        public GastosViewModel Gastos { get; set; }
        public IngresosViewModel Ingresos { get; set; }
        public BalanceViewModel Balance { get; set; }
        public EditViewModel Edit { get; set; }
        public DeleteViewModel Delete { get; set; }
        public EditarGastosViewModel EditarGastos { get; set; }
        public BorrarGastosViewModel BorrarGastos { get; set; }
        public EditarIngresosViewModel EditarIngresos { get; set; }
        public BorrarIngresosViewModel BorrarIngresos { get; set; }
        public InfoViewModel Info { get; set; }
        public NotificationViewModel Notification { get; set; }
        public EditarNotificationViewModel EditarNotification { get; set; }
        public BorrarNotificationViewModel BorrarNotification { get; set; }
        public BalanceGeneralViewModel BalanceGeneral { get; set; }
        public BorrarBalanceGeneralViewModel BorrarBalanceGeneral { get; set; }
        #endregion


        #region Constructor
        public MainViewModel()
        {
            instance = this;
            //PaginaInicio = new PaginaInicioViewModel();
        }
        #endregion

        #region Singleton

        static MainViewModel instance;

        public static MainViewModel GetInstance()
        {
            if (instance == null)
            {
                return new MainViewModel();
            }
            return instance;
        }
        #endregion

    }
}
