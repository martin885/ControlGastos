using SQLite.Net.Interop;

namespace ControlGastos.Interfaces
{
    public interface IConfig
    {
         string DirectoryDB { get;  }
         ISQLitePlatform Platform { get;}

    }
}
