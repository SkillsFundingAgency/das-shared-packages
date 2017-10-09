namespace SFA.DAS.HashingService
{
    public interface IHashingService
    {
        string HashValue(long id);
        long DecodeValue(string id);
    }
}
