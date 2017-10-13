using System;

namespace SFA.DAS.HashingService
{
    public interface IHashingService
    {
        string HashValue(long id);
        string HashValue(Guid id);

        long DecodeValue(string id);
        Guid DecodeValueToGuid(string id);
    }
}
