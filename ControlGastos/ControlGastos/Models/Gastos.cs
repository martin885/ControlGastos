using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
