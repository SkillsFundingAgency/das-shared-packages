using System;
using System.Linq.Expressions;

namespace SFA.DAS.Validation
{
    public class ValidationException<T> : ValidationException
    {
        public ValidationException(Expression<Func<T, object>> expression, string message)
            : base(expression, message)
        {
        }
    }

    public class ValidationException : Exception
    {
        public LambdaExpression Expression { get; }

        public ValidationException(string message)
            : this(null, message)
        {
        }

        public ValidationException(LambdaExpression expression, string message)
            : base(message)
        {
            Expression = expression;
        }
    }
}