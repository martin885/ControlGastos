using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.Net.Interop;
using Windows.Storage;
using Xamarin.Forms;
using ControlGastos.Interfaces;

[assembly: Dependency(typeof(ControlGastos.UWP.Implementations.Config))]

//Agregar siempre la referencia para poder usar la base de datos, sino aparece una excepción
namespace ControlGastos.UWP.Implementations
{
   public class Config : IConfig
    {
        private string directoryDB;
        private ISQLitePlatform platform;

        public string DirectoryDB
        {
            get
            {
                if (string.IsNullOrEmpty(directoryDB))
                {
                    directoryDB = ApplicationData.Current.LocalFolder.Path;
                }
                return directoryDB;
            }
        }

        public ISQLitePlatform Platform
        {
            get
            {
                if (platform == null)
                {
                    platform = new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT();
                }
                return platform;
            }
        }
    }
}
