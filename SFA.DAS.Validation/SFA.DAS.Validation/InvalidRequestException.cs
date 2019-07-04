using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Validation
{
    public class InvalidRequestException : Exception
    {
        public Dictionary<string, string> ErrorMessages { get; }

        public InvalidRequestException(Dictionary<string, string> errorMessages)
            : base(BuildErrorMessage(errorMessages))
        {
            ErrorMessages = errorMessages;
        }

        private static string BuildErrorMessage(Dictionary<string, string> errorMessages)
        {
            if (!errorMessages.Any())
            {
                return "Request is invalid";
            }

            return "Request is invalid:\n" + errorMessages.Select(kvp => $"{kvp.Key}: {kvp.Value}").Aggregate((x, y) => $"{x}\n{y}");
        }
    }
}
