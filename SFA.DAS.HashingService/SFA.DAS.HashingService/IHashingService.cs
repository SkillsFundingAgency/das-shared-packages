using System;

namespace SFA.DAS.HashingService
{
    public interface IHashingService
    {
        string HashValue(long id);
        string HashValue(Guid id);
        string HashValue(string id);
        long DecodeValue(string id);
        Guid DecodeValueToGuid(string id);
        string DecodeValueToString(string id);
        bool TryDecodeValue(string input, out long output);
    }
}
