using System;
namespace SFA.DAS.NLog.Logger
{
    public interface IRequestContext
    {
        string Url { get; }
        string IpAddress { get; }
        string ApplicationName { get; }
    }
}
