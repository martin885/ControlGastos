using ControlGastos.ViewModels;
using ControlGastos.Views;
using GalaSoft.MvvmLight.Command;
using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ControlGastos.Models
{
    public class Gastos
    {
        [PrimaryKey, AutoIncrement]
        public int GastosId { get; set; }

        public string GastosCantidad { get; set; }

        public string Anio { get; set; }

        public string Mes { get; set; }

        public string Dia { get; set; }

        public string GastoNombre { get; set; }

        public string Categoria{ get; set; }

        public string ImagenFecha { get; set; }

        public string ImagenOrigen { get; set; }

        public string ImagenMonto { get; set; }

        public override int GetHashCode()
        {
            return GastosId;
        }

        #region Commands
        public ICommand EditCommand
        {
            get
            {
                return new RelayCommand(Edit);
            }
        }
        private void Edit()
        {
            //Instanciar ViewModel
            var mainViewModel = MainViewModel.GetInstance();
            mainViewModel.EditarGastos = new EditarGastosViewModel(this);
           
            //Ir a la página de edición
            var gastosView = GastosView.GetInstance();
            gastosView.Navigation.PushAsync(new EditarGastos());
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
            mainViewModel.BorrarGastos = new BorrarGastosViewModel(this);
        }
        #endregion
    }
}
