namespace SFA.DAS.Authorization
{
    public interface ICallerContextProvider
    {
        ICallerContext GetCallerContext();
    }
}