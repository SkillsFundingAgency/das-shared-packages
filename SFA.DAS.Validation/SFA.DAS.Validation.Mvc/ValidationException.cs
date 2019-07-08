#if NET462
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;

namespace SFA.DAS.Validation.Mvc
{
    public class ValidationException<T> : ValidationException
    {
        public ValidationException(Expression<Func<T, object>> expression, string message)
            : base(typeof(T), ExpressionHelper.GetExpressionText(expression), message)
        {
        }
    }
}
#endif