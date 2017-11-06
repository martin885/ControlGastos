using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ControlGastos.Models
{
    public class Balance
    {
        public string Cantidad { get; set; }

        public string Fecha { get; set; }

        public string Origen { get; set; }

        public string GastoIngreso { get; set; }

        public Color ColorGastoIngreso { get; set; }
    }
}

