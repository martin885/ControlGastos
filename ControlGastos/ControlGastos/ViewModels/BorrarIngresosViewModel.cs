using ControlGastos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlGastos.ViewModels
{
  public  class BorrarIngresosViewModel
    {
        private Ingresos ingresos;

        public BorrarIngresosViewModel(Ingresos ingresos)
        {
            this.ingresos = ingresos;
            var ingresosViewModel = IngresosViewModel.GetInstance();
             ingresosViewModel.Delete(this.ingresos);
        }
    }
}
