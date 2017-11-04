

namespace ControlGastos.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Data;

   //Librerias que se usan: SQLite.Net-PCL y SQLiteNetExtensions Agregar a cada proyecto

    public class DataService
    {
        public bool DeleteAll<T>() where T : class
        {
            try
            {
                using (var da = new DataAccess())
                {
                    da.DeleteAll<T>();
                }

                return true;
            }
            catch (Exception ex)
            {
                ex.ToString();
                return false;
            }
        }

        public T DeleteAllAndInsert<T>(T model,bool WithChildren) where T : class
        {
            try
            {
                using (var da = new DataAccess())
                {
                    var oldRecords = da.GetList<T>(false);
                    foreach (var oldRecord in oldRecords)
                    {
                        da.Delete(oldRecord);
                    }

                    da.Insert(model,WithChildren);

                    return model;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return model;
            }
        }

        public T InsertOrUpdate<T>(T model,bool WithChildren) where T : class
        {
            try
            {
                using (var da = new DataAccess())
                {
                    var oldRecord = da.Find<T>(model.GetHashCode(), false);
                    if (oldRecord != null)
                    {
                        da.Update(model, WithChildren);
                    }
                    else
                    {
                        da.Insert(model,WithChildren);
                    }

                    return model;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                return model;
            }
        }

        public T Insert<T>(T model,bool WithChildren)
        {
            using (var da = new DataAccess())
            {
                da.Insert(model,WithChildren);
                return model;
            }
        }

        public T Find<T>(int pk, bool withChildren) where T : class
        {
            using (var da = new DataAccess())
            {
                return da.Find<T>(pk, withChildren);
            }
        }

        public T First<T>(bool withChildren) where T : class
        {
            using (var da = new DataAccess())
            {
                return da.GetList<T>(withChildren).FirstOrDefault();
            }
        }

        public List<T> Get<T>(bool withChildren) where T : class
        {
            using (var da = new DataAccess())
            {
                return da.GetList<T>(withChildren).ToList();
            }
        }

        public bool CheckTableIsEmpty<T>() where T : class
        {
            using (var da = new DataAccess())
            {
                return da.CheckTableIsEmpty<T>();
            }
        }

        public void Update<T>(T model,bool WithChildren)
        {
            using (var da = new DataAccess())
            {
                da.Update(model, WithChildren);
            }
        }

        public void Delete<T>(T model)
        {
            using (var da = new DataAccess())
            {
                da.Delete(model);
            }
        }

        public void Save<T>(List<T> list,bool WithChildren) where T : class
        {
            using (var da = new DataAccess())
            {
                foreach (var record in list)
                {
                    InsertOrUpdate(record, WithChildren);
                }
            }
        }
    }
}
