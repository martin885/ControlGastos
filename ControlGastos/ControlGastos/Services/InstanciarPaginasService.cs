using ControlGastos.ViewModels;
using ControlGastos.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlGastos.Services
{
    class InstanciarPaginasService
    {
        public void Instanciar()
        {
            BalanceGeneralView.GetInstance().initialize();
            BalanceView.GetInstance().initialize();
            GastosView.GetInstance().initialize();
            IngresosView.GetInstance().initialize();
        }
    }
}
