namespace SFA.DAS.Events.Api.Types
{
    public interface IEventView
    {
        long Id { get; set; }

        string Event { get; set; }
    }
}
