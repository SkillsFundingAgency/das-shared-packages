using System;

namespace SFA.DAS.Commitments.Domain.Interfaces
{
    public interface ICurrentDateTime
    {
        DateTime Now { get; }
    }
}
