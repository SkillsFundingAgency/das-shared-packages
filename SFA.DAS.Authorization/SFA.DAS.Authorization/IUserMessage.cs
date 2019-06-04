using System;

namespace SFA.DAS.Authorization
{
    public interface IUserMessage
    {
        Guid? UserRef { get; set; }
    }
}