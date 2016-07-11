namespace SFA.DAS.Messaging.Syndication.Hal
{
    public interface IHalResourceAttributeExtrator<T>
    {
        HalResourceAttributes Extract(T resource);
    }
}
