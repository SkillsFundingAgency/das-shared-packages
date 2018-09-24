using System.Linq.Expressions;

namespace SFA.DAS.Validation
{
    public class ValidationError
    {
        public object Instance { get; }
        public LambdaExpression Property { get; }
        public string Message { get; }

        public ValidationError(object instance, LambdaExpression property, string message)
        {
            Instance = instance;
            Property = property;
            Message = message;
        }
    }
}