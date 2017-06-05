using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Finix.IPDC.Infrastructure
{

    [Serializable]
    public class UnitOfWorkException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        internal UnitOfWorkException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        public UnitOfWorkException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class UnitOfWorkUpdateException : UnitOfWorkException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkUpdateException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public UnitOfWorkUpdateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkUpdateException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        public UnitOfWorkUpdateException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }


    public class UnitOfWorkValidationException : UnitOfWorkException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkValidationException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        internal UnitOfWorkValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkValidationException"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="streamingContext">The streaming context.</param>
        public UnitOfWorkValidationException(SerializationInfo info, StreamingContext streamingContext)
            : base(info, streamingContext)
        {
        }

        /// <summary>
        /// Gets the validation errors.
        /// </summary>
        /// <value>
        /// The validation errors.
        /// </value>
        public IEnumerable<UnitOfWorkEntityValidationError> ValidationErrors
        {
            get
            {
                var innerException = InnerException as DbEntityValidationException;
                if (innerException == null)
                    return Enumerable.Empty<UnitOfWorkEntityValidationError>();

                return innerException
                    .EntityValidationErrors
                    .Where(x => !x.IsValid)
                    .Select(x => new UnitOfWorkEntityValidationError(x));
            }
        }
    }

    public class UnitOfWorkEntityValidationError
    {
        private readonly DbEntityValidationResult dbEntityValidationResult;

        internal UnitOfWorkEntityValidationError(DbEntityValidationResult dbEntityValidationResult)
        {
            this.dbEntityValidationResult = dbEntityValidationResult;
        }

        public object Entity
        {
            get
            {
                return dbEntityValidationResult.Entry.Entity;
            }
        }

        public IReadOnlyCollection<UnitOfWorkPropertyValidationError> ValidationErrors
        {
            get
            {
                return dbEntityValidationResult
                    .ValidationErrors
                    .Select(x => new UnitOfWorkPropertyValidationError(x))
                    .ToList();
            }
        }
    }

    public class UnitOfWorkPropertyValidationError
    {
        private readonly DbValidationError dbValidationError;

        internal UnitOfWorkPropertyValidationError(DbValidationError dbValidationError)
        {
            this.dbValidationError = dbValidationError;
        }

        public string PropertyName
        {
            get { return dbValidationError.PropertyName; }
        }

        public string ErrorMessage
        {
            get { return dbValidationError.ErrorMessage; }
        }
    }

}
