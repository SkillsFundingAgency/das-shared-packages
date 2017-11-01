using System;
using SFA.DAS.Commitments.Domain.Interfaces;

namespace SFA.DAS.Commitments.Infrastructure.Services
{
    public class CurrentDateTime : ICurrentDateTime
    {
        private readonly DateTime? _time;

        public DateTime Now => _time ?? DateTime.UtcNow;

        public CurrentDateTime()
        {
        }

        public CurrentDateTime(DateTime? time)
        {
            _time = time;
        }
    }
}
