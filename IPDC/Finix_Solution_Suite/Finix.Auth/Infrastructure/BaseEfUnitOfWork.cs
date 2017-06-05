using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.Auth.Infrastructure
{
    public abstract class BaseEfUnitOfWork : IUnitOfWork
    {
        private DbContext context;

        protected BaseEfUnitOfWork(DbContext context)
        {
            this.context = context;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IQueryable<T> GetSet<T>() where T : class
        {
            try
            {
                return context.Set<T>();
            }
            catch (Exception ex)
            {
                throw new UnitOfWorkException("BaseEfUnitOfWork.GetSet", ex);
            }
        }

        public IEnumerable<T> GetChangedEntities<T>() where T : class
        {
            var entries = context.ChangeTracker.Entries<T>()
                .Where(t => t.State == EntityState.Modified || t.State == EntityState.Added)
                .Select(t => t.Entity);
            return entries;
        }

        public int Save()
        {
            try
            {
                return context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                var message = ExtractDbEntityValidationExceptionMessage(ex);

                throw new UnitOfWorkValidationException(
                    string.Format("BaseEfUnitOfWork.Save - {0}", message), ex);
            }
            catch (DbUpdateException ex)
            {
                throw new UnitOfWorkUpdateException("BaseEfUnitOfWork.Save", ex);
            }
            catch (Exception ex)
            {
                throw new UnitOfWorkException("BaseEfUnitOfWork.Save", ex);
            }
        }


        public void Add<T>(T entity) where T : class
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity", "Entity is null.");
            }


            try
            {
                context.Set<T>().Add(entity);
            }
            catch (Exception ex)
            {
                throw new UnitOfWorkException("BaseEfUnitOfWork.Add", ex);
            }
        }


        public void AddRange<T>(IEnumerable<T> entities) where T : class
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities", "Entities is null.");
            }

            try
            {
                context.Set<T>().AddRange(entities);
            }
            catch (Exception ex)
            {
                throw new UnitOfWorkException("BaseEfUnitOfWork.AddRange", ex);
            }
        }

        public void Update<T>(T entity) where T : class
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity", "Entity is null.");
            }

            // Nothing to do -- EF keeps track of changed entities
        }

        public void Remove<T>(T entity) where T : class
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity", "Entity is null.");
            }

            try
            {
                context.Set<T>().Remove(entity);
            }
            catch (Exception ex)
            {
                throw new UnitOfWorkException("BaseEfUnitOfWork.Remove", ex);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            context.Dispose();
        }

        private static string ExtractDbEntityValidationExceptionMessage(DbEntityValidationException ex)
        {
            try
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " Validation errors: ", fullErrorMessage);
                return exceptionMessage;
            }
            catch (Exception)
            {
                return "Could not extract DbEntityValidationExceptionMessage";
            }
        }
    }



    public class FinixAuthUnitOfWork : BaseEfUnitOfWork
    {
        public FinixAuthUnitOfWork(FinixAuthContext context)
            : base(context)
        {

        }
    }
}
