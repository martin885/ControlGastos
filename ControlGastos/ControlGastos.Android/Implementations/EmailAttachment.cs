using Plugin.Messaging;
using System;
using Xamarin.Forms;
using ControlGastos.Interfaces;

[assembly: Dependency(typeof(ControlGastos.Droid.Implementations.EmailAttachment))]

namespace ControlGastos.Droid.Implementations
{
    public class EmailAttachment : IEmail
    {


        public IEmailMessage EmailMessage(string filename)
        {


                string root = null;
                //Get the root path in android device.
                if (Android.OS.Environment.IsExternalStorageEmulated)
                {
                    root = Android.OS.Environment.ExternalStorageDirectory.ToString();
                }
                else
                    root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                //Create directory and file 
                Java.IO.File myDir = new Java.IO.File(root + "/Balances");
                myDir.Mkdir();

                Java.IO.File file = new Java.IO.File(myDir, string.Format("{0}.xlsx", filename));

            if (file.Exists())
            {
                var emailConAdjunto = new EmailMessageBuilder()
                      .Subject(string.Format("Balance: {0}", filename))
                      .Body(string.Format("Envío el archivo con la hoja de cálculos del balance de {0}. Saludos", filename))
                      .WithAttachment(file)
                      .Build();

                return emailConAdjunto;
            }
            else { 
            var email=  new EmailMessageBuilder()
                      .Build();

                return email;

            }


        }
    }
}