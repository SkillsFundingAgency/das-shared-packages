using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.Events.Dispatcher.UnitTests.EventDispatcherTests
{
    public class OtherTestEvent : IEventView
    {
        public long Id { get; set; }
        public string Event { get; set; }
    }
}
