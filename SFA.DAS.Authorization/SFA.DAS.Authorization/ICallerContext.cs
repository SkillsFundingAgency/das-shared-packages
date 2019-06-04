using System;

namespace SFA.DAS.Authorization
{
    public interface ICallerContext
    {
        string AccountHashedId { get; }
        long? AccountId { get; }
        Guid? UserRef { get; }
    }
}