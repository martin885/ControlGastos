using Plugin.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ControlGastos.Interfaces
{
   
        public interface IEmail
        {
            IEmailMessage EmailMessage(string filename);
        }
    
}
