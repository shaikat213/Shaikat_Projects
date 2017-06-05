using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Finix.IPDC.Infrastructure;

namespace Finix.IPDC.Service
{

    public interface IFinixIPDCService<T> where T : Entity
    {
        void Save(T entity);
        void Save(List<T> entityList);
        void Delete(T entity);
        void Delete(long id);

        IQueryable<T> GetAll();
        T GetById(long id);
        T GetSingleById(long id);

        int SaveChanges();
    }

    public class FinixIPDCService<T> : IFinixIPDCService<T> where T : Entity
    {
        private readonly IUnitOfWork uow;

        public FinixIPDCService(IUnitOfWork uow)
        {
            this.uow = uow;
        }
        public IQueryable<T> GetAll()
        {
            return uow.GetSet<T>();
        }
        public T GetById(long id)
        {
            return uow.GetSet<T>().FirstOrDefault(x => x.Id == id);
        }
        public T GetSingleById(long id)
        {
            return uow.GetSet<T>().Single(x => x.Id == id);
        }

        public void Save(T entity)
        {
            uow.Add(entity);
            SaveChanges();
        }
        public void Save(List<T> entityList)
        {
            uow.AddRange(entityList);
            SaveChanges();
        }

        public void Delete(T entity)
        {
            uow.Remove(entity);
            SaveChanges();
        }

        public void Delete(long id)
        {
            uow.Remove(GetSingleById(id));
            SaveChanges();
        }

        public int SaveChanges()
        {
            return uow.Save();
        }
    }


    #region gen service
    public class GenService
    {
        private readonly IUnitOfWork uow;

        public GenService()
        {
            uow = new FinixIPDCUnitOfWork(new FinixIPDCContext());
        }
        public IEnumerable<T> QueryOver<T>(Expression<Func<T, bool>> where) where T : Entity
        {
            return uow.GetSet<T>().Where(where).ToList();
        }
        public IQueryable<T> GetAll<T>() where T : Entity
        {
            return uow.GetSet<T>();
        }
        public T GetById<T>(long id) where T : Entity
        {
            return uow.GetSet<T>().FirstOrDefault(x => x.Id == id);
        }
        public T GetSingleById<T>(long id) where T : Entity
        {
            return uow.GetSet<T>().Single(x => x.Id == id);
        }

        public void Save<T>(T entity, bool persistChangesToDb = true) where T : Entity
        {
            if (entity.Id > 0)
                uow.Update(entity);
            else
                uow.Add(entity);
            if (persistChangesToDb)
                SaveChanges();
        }
        public void Save<T>(List<T> entityList, bool persistChangesToDb = true) where T : Entity
        {
            /*Sabiha Modified*/
            foreach (var entity in entityList)
            {
                if (entity.Id > 0)
                    uow.Update(entity);
                else
                    uow.Add(entity);
                if (persistChangesToDb)
                    SaveChanges();
            }
            /*Sabiha Modified*/

            //uow.AddRange(entityList);
            //if (persistChangesToDb)
            //    SaveChanges();
        }

        public void Delete<T>(T entity, bool persistChangesToDb = true) where T : Entity
        {
            uow.Remove(entity);
            if (persistChangesToDb)
                SaveChanges();
        }

        public void Delete<T>(long id, bool persistChangesToDb = true) where T : Entity
        {
            uow.Remove(GetSingleById<T>(id));
            if (persistChangesToDb)
                SaveChanges();
        }

        public int SaveChanges()
        {
            return uow.Save();
        }
    }
    #endregion
}
