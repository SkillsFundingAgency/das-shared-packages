using System;
using Microsoft.Azure.WebJobs.Description;

namespace SFA.DAS.NServiceBus.AzureFunction.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    [Binding]
    public sealed class NServiceBusTriggerAttribute : Attribute
    {
        public string Endpoint { get; set; }
        public string Connection { get; set; }
        public string LearningTransportStorageDirectory { get; set; }
    }
}
