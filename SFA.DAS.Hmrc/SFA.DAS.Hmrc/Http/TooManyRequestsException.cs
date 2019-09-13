namespace SFA.DAS.Hmrc.Http
{
    public class TooManyRequestsException : HttpException
    {
        public TooManyRequestsException()
            : base(429, "Rate limit has been reached")
        {
        }
    }
}