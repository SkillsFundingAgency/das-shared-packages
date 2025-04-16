namespace SFA.DAS.DfESignIn.Auth.Api.Models
{
    public class ApiResponse<T>(T body)
    {
        public T Body { get; set; } = body;
    }
} 