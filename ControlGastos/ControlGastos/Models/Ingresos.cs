using ControlGastos.ViewModels;
using ControlGastos.Views;
using GalaSoft.MvvmLight.Command;
using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ControlGastos.Models
{
   public class Ingresos
    {
        [PrimaryKey, AutoIncrement]
        public int IngresoId { get; set; }

        //[ForeignKey(typeof(IngresosMes))]
        //public int IngresosMesId { get; set; }

        public string IngresoCantidad { get; set; }

        public string Anio { get; set; }

        public string Mes { get; set; }

        public string Dia { get; set; }

        public string IngresoNombre { get; set; }

        public string ImagenFecha { get; set; }

        public string ImagenOrigen { get; set; }

        public string ImagenMonto { get; set; }

        public override int GetHashCode()
        {
            return IngresoId;
        }

        #region Commands
        public ICommand EditCommand
        {
            get
            {
                return new RelayCommand(Edit);
            }
        }
        private async void Edit()
        {
            //Instanciar ViewModel
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.EditarIngresos = new EditarIngresosViewModel(this);

            //Ir a la página de edición
            var ingresosView = IngresosView.GetInstance();
          await  ingresosView.Navigation.PushAsync(new EditarIngresosView());
        }

        public ICommand DeleteCommand
        {
            get
            {
                return new RelayCommand(Delete);
            }
        }


        private void Delete()
        {
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.BorrarIngresos = new BorrarIngresosViewModel(this);
        }
        #endregion

    }
}
