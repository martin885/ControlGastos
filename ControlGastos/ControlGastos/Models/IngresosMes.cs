using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlGastos.Models
{
    public class IngresosMes
    {
        [PrimaryKey, AutoIncrement]
        public int IngresosMesId { get; set; }

        [OneToMany(CascadeOperations = CascadeOperation.All)]
        public List<Ingresos> IngresoLista { get; set; }

        public override int GetHashCode()
        {
            return IngresosMesId;
        }
    }
}
