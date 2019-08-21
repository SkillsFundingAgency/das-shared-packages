using System;
using Microsoft.Azure.WebJobs.Description;

namespace SFA.DAS.NServiceBus.AzureFunction.Infrastructure
{
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public sealed class NServiceBusTriggerAttribute : Attribute
    {
        public string EndPoint { get; set; }
        public string Connection { get; set; }
    }
}
