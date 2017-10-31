using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlGastos.Models
{
    public class IngresosTotales
    {
        public int IngresosTotalesId { get; set; }

        public string IngresoMes { get; set; }

        public string IngresoTotal { get; set; }

        public List<Ingresos> IngresoLista { get; set; }
    }
}
