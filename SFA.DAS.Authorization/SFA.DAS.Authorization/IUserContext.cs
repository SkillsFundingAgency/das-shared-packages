using System;

namespace SFA.DAS.Authorization
{
    public interface IUserContext
    {
        long Id { get; }
        Guid Ref { get; }
        string Email { get; }
    }
}