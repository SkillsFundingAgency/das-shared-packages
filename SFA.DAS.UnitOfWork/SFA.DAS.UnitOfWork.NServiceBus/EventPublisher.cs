using System;
using System.Threading.Tasks;
using SFA.DAS.NServiceBus;

namespace SFA.DAS.UnitOfWork.NServiceBus
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IUnitOfWorkContext _unitOfWorkContext;

        public EventPublisher(IUnitOfWorkContext unitOfWorkContext)
        {
            _unitOfWorkContext = unitOfWorkContext;
        }

        public Task Publish<T>(T message) where T : Event
        {
            _unitOfWorkContext.AddEvent(message);

            return Task.CompletedTask;
        }

        public Task Publish<T>(Func<T> messageFactory) where T : Event
        {
            _unitOfWorkContext.AddEvent(messageFactory);

            return Task.CompletedTask;
        }
    }
}