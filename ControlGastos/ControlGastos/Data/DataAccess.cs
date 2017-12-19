using ControlGastos.Interfaces;
using ControlGastos.Models;
using SQLite.Net;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xamarin.Forms;

//Librerias que se usan: SQLite.Net-PCL y SQLiteNetExtensions Agregar a cada proyecto

namespace ControlGastos.Data

{
    public class DataAccess : IDisposable
    {
        SQLiteConnection connection;

        public DataAccess()
        {
            var config = DependencyService.Get<IConfig>();
            connection = new SQLiteConnection(config.Platform,
            Path.Combine(config.DirectoryDB, "ControlGastos.db3"));
            //connection.DropTable<IngresosMes>();
            //connection.CreateTable<Gastos>();
            //connection.DropTable<Ingresos>();
            connection.CreateTable<Ingresos>();
            connection.CreateTable<Gastos>();
            connection.CreateTable<Notification>();
            connection.CreateTable<NotificacionDiaria>();
            //connection.DropTable<Notification>();
            //connection.CreateTable<SemanasA>();

        }

        public void Insert<T>(T model, bool WithChildren)
        {
            if (WithChildren)
            {
                connection.InsertWithChildren(model);
            }
            else { 
            connection.Insert(model);
            }
        }

        public void Update<T>(T model,bool WithChildren)
        {
            if (WithChildren) { 
                connection.UpdateWithChildren(model);
            }
           else
            {
                connection.Update(model);
            }
        }


        public void Delete<T>(T model)
        {
            connection.Delete(model);
        }
        

        public void DeleteAll<T>()
        {
            connection.DeleteAll<T>();
        }

        public T First<T>(bool WithChildren) where T : class
        {
            if (WithChildren)
            {
                return connection.GetAllWithChildren<T>().FirstOrDefault();
            }
            else
            {
                return connection.Table<T>().FirstOrDefault();
            }
        }

        public List<T> GetList<T>(bool WithChildren) where T : class
        {
            if (WithChildren)
            {
                return connection.GetAllWithChildren<T>().ToList();
            }
            else
            {
                return connection.Table<T>().ToList();
            }
        }

        public bool CheckTableIsEmpty<T>() where T : class
        {

            return connection.Table<T>().Any();
         
        }

        public T Find<T>(int pk, bool WithChildren) where T : class
        {
            if (WithChildren)
            {
                return connection
                    .GetAllWithChildren<T>()
                    .FirstOrDefault(m => m.GetHashCode() == pk);
            }
            else
            {
                return connection
                    .Table<T>()
                    .FirstOrDefault(m => m.GetHashCode() == pk);
            }
        }

        public void Dispose()
        {
            connection.Dispose();
        }
    }
}
