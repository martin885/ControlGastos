using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
