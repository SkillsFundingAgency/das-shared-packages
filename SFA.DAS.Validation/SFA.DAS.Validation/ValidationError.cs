using System.Linq.Expressions;

namespace SFA.DAS.Validation
{
    public class ValidationError
    {
        public LambdaExpression Property { get; }
        public string Message { get; }

        public ValidationError(LambdaExpression property, string message)
        {
            Property = property;
            Message = message;
        }
    }
}