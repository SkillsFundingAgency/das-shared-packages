using System.Linq.Expressions;

namespace SFA.DAS.Validation
{
    public class ValidationError
    {
        public object Model { get; }
        public LambdaExpression Property { get; }
        public string Message { get; }

        public ValidationError(object model, LambdaExpression property, string message)
        {
            Model = model;
            Property = property;
            Message = message;
        }
    }
}