using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlGastos.Models
{
    public class Notification
    {
        [PrimaryKey, AutoIncrement]
        public int NotificationId { get; set; }

        public string Title { get; set; }

        public string Hora { get; set; }

        public string Minutos { get; set; }

        public TimeSpan TiempoRestanteEnvio { get; set; }

        public string Anio { get; set; }

        public string Mes { get; set; }

        public string Dia { get; set; }

        public string Message { get; set; }

        public override int GetHashCode()
        {
            return NotificationId;
        }
    }
}
