using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace SFA.DAS.Validation
{
    public class ValidationException<T> : ValidationException
    {
        public ValidationException(Expression<Func<T, object>> expression, string message)
            : base(typeof(T), ExpressionHelper.GetExpressionText(expression), message)
        {
        }
    }

    public class ValidationException : Exception
    {
        public IEnumerable<ValidationError> ValidationErrors => _validationErrors;

        private readonly List<ValidationError> _validationErrors = new List<ValidationError>();

        public Type MessageType { get; protected set; }
        public string PropertyName { get; protected set; }

        public ValidationException()
            : base("")
        {
        }

        public ValidationException(string message)
            : base(message)
        {
        }

        public ValidationException(Type messageType, string propertyName, string message)
            : base(message)
        {
            MessageType = messageType;
            PropertyName = propertyName;
        }

        public ValidationException AddError<T>(Expression<Func<T, object>> property, string message) where T : class
        {
            _validationErrors.Add(new ValidationError(property, message));

            return this;
        }
    }
}