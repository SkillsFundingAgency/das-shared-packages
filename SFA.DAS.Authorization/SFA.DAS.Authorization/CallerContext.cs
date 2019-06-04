using System;

namespace SFA.DAS.Authorization
{
    public class CallerContext : ICallerContext
    {
        public string AccountHashedId { get; set; }
        public long? AccountId { get; set; }
        public Guid? UserRef { get; set; }
    }
}