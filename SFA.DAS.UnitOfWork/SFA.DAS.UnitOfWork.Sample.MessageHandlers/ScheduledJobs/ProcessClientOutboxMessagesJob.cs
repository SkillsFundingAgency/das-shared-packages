using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using SFA.DAS.NServiceBus.ClientOutbox;

namespace SFA.DAS.UnitOfWork.Sample.MessageHandlers.ScheduledJobs
{
    public class ProcessClientOutboxMessagesJob
    {
        private readonly IProcessClientOutboxMessagesJob _processClientOutboxMessagesJob;

        public ProcessClientOutboxMessagesJob(IProcessClientOutboxMessagesJob processClientOutboxMessagesJob)
        {
            _processClientOutboxMessagesJob = processClientOutboxMessagesJob;
        }

        [Singleton]
        public Task Run([TimerTrigger("0 */10 * * * *", RunOnStartup = true)] TimerInfo timer)
        {
            return _processClientOutboxMessagesJob.RunAsync();
        }
    }
}