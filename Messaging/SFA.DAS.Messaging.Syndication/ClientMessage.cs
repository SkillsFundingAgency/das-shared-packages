namespace SFA.DAS.Messaging.Syndication
{
    public class ClientMessage<T>
    {
        public string Identifier { get; set; }
        public T Message { get; set; }
    }
}