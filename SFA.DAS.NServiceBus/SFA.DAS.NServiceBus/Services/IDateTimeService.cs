using System;

namespace SFA.DAS.NServiceBus.Services
{
    public interface IDateTimeService
    {
        DateTime UtcNow { get; }
    }
}