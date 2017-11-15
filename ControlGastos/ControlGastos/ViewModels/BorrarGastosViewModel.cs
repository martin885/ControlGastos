using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ControlGastos.Models;

namespace ControlGastos.ViewModels
{
   public class BorrarGastosViewModel
    {
                   
           
        private Gastos gastos;

        public BorrarGastosViewModel(Gastos gastos)
        {
            this.gastos = gastos;
            var gastosViewModel = GastosViewModel.GetInstance();
            gastosViewModel.Delete(this.gastos);
        }

        
        }
}
