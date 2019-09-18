namespace SFA.DAS.Hmrc.Http
{
    public class ServiceUnavailableException : HttpException
    {
        public ServiceUnavailableException()
            : base(503, "Service is unavailable")
        {
        }
    }
}