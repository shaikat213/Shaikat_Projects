using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Gets a set of T.
        /// </summary>
        /// <typeparam name="T">Type of set to return.</typeparam>
        /// <returns>Returns a set of T.</returns>
        IQueryable<T> GetSet<T>() where T : class;

        /// <summary>
        /// Gets the changed entities.
        /// </summary>
        /// <typeparam name="T">Type of set to return.</typeparam>
        /// <returns>Returns a set of T.</returns>
        IEnumerable<T> GetChangedEntities<T>() where T : class;

        /// <summary>
        /// Persists changes to the data store.
        /// </summary>
        /// <returns>Number of objects affected.</returns>
        int Save();

        /// <summary>
        /// Adds T.
        /// </summary>
        /// <typeparam name="T">Type of the entity to add.</typeparam>
        /// <param name="entity">The entity to add.</param>
        void Add<T>(T entity) where T : class;

        /// <summary>
        /// Adds multiple T.
        /// </summary>
        /// <typeparam name="T">Type of the entity to add.</typeparam>
        /// <param name="entities">The entities to add.</param>
        void AddRange<T>(IEnumerable<T> entities) where T : class;

        /// <summary>
        /// Updates the entity.
        /// </summary>
        /// <typeparam name="T">Type of the entity to update.</typeparam>
        /// <param name="entity">The entity to update.</param>
        void Update<T>(T entity) where T : class;

        /// <summary>
        /// Removes T.
        /// </summary>
        /// <typeparam name="T">Type of the entity to remove.</typeparam>
        /// <param name="entity">The entity to remove.</param>
        void Remove<T>(T entity) where T : class;
    }

}
