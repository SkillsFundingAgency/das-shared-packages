namespace SFA.DAS.NLog.Logger
{
    public interface IWebLoggingContext : ILoggingContext
    {
        string IpAddress { get; }

        string Url { get; }
    }
}