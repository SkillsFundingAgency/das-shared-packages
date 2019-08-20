using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SFA.DAS.Validation.Exceptions
{
    public class ValidationException : Exception
    {
        public IEnumerable<ValidationError> ValidationErrors => _validationErrors;

        private readonly List<ValidationError> _validationErrors = new List<ValidationError>();

        public ValidationException()
            : base("")
        {
        }

        public ValidationException(string message)
            : base(message)
        {
        }

        public ValidationException AddError<T>(Expression<Func<T, object>> property, string message) where T : class
        {
            _validationErrors.Add(new ValidationError(property, message));

            return this;
        }
    }
}