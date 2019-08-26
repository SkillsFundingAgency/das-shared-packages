using System;
using System.Threading.Tasks;
using SFA.DAS.NServiceBus.Services;
using SFA.DAS.UnitOfWork.Context;

namespace SFA.DAS.UnitOfWork.NServiceBus.Services
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IUnitOfWorkContext _unitOfWorkContext;

        public EventPublisher(IUnitOfWorkContext unitOfWorkContext)
        {
            _unitOfWorkContext = unitOfWorkContext;
        }

        public Task Publish<T>(T message) where T : class
        {
            _unitOfWorkContext.AddEvent(message);

            return Task.CompletedTask;
        }

        public Task Publish<T>(Func<T> messageFactory) where T : class
        {
            _unitOfWorkContext.AddEvent(messageFactory);

            return Task.CompletedTask;
        }
    }
}