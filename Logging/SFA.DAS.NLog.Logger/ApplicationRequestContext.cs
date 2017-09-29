namespace SFA.DAS.NLog.Logger
{
    public class ApplicationRequestContext : BaseRequestContext
    {
        public ApplicationRequestContext(string applicationName)
            : base(null, null, applicationName) { }
    }
}
