namespace SFA.DAS.NLog.Logger
{
    public class ConsoleRequestContext : BaseRequestContext
    {
        public ConsoleRequestContext(string applicationName)
            : base(null, null, applicationName) { }
    }
}
