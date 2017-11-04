using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlGastos.Models
{
    public class IngresosAño
    {
        public int IngresosAñoId { get; set; }

        public List<IngresosMes> IngresosMesLista;
    }
}

