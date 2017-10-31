using ControlGastos.Models;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ControlGastos.ViewModels
{
    public class IngresosViewModel
    {

        #region Propiedades y Atributos
        IngresosTotales IngresosTotales { get; set; }
        Ingresos Ingresos { get; set; }
        #endregion

        #region Commands
        public ICommand AgregarIngresoCommand
        {
            get
            {
                return new RelayCommand(AgregarIngreso);
            }
        }

        private void AgregarIngreso()
        {

        }
        #endregion

        #region Constructor
        public IngresosViewModel()
        {
    
        }
        #endregion

    }
}
