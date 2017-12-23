using SQLite.Net.Attributes;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlGastos.Models
{
   public class NotificacionDiaria
    {

        #region Propiedades
        [PrimaryKey, AutoIncrement]
        public int NotificacionDiariaId { get; set; }

        [ForeignKey(typeof(Notification))]
        public int NotificationId { get; set; }

        [ManyToOne]
        public Notification notification { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public DateTime Fecha { get; set; }

        public TimeSpan Horario { get; set; }

        public bool TodosLosDiasActivado { get; set; }

        public override int GetHashCode()
        {
            return NotificationId;
        }
        #endregion
       }
    }
