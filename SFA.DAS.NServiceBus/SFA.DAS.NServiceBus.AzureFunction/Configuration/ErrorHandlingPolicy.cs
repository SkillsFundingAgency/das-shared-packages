using System.Threading;
using System.Threading.Tasks;
using NServiceBus.Raw;
using NServiceBus.Transport;

namespace SFA.DAS.NServiceBus.AzureFunction.Configuration
{
    public class ErrorHandlingPolicy : IErrorHandlingPolicy
    {
        private readonly string _poisonMessageQueue;
        private readonly int _immediateRetryCount;

        public ErrorHandlingPolicy(string poisonMessageQueue, int immediateRetryCount)
        {
            _poisonMessageQueue = poisonMessageQueue;
            _immediateRetryCount = immediateRetryCount;
        }

        public Task<ErrorHandleResult> OnError(IErrorHandlingPolicyContext handlingContext, IMessageDispatcher dispatcher, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (handlingContext.Error.ImmediateProcessingFailures < _immediateRetryCount)
            {
                return Task.FromResult(ErrorHandleResult.RetryRequired);
            }

            return handlingContext.MoveToErrorQueue(_poisonMessageQueue, attachStandardFailureHeaders: true, cancellationToken);
        }
    }
}