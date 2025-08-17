using System;
using System.Collections.Generic;
using NServiceBus.Transport;

namespace SFA.DAS.NServiceBus.AzureFunction.Hosting
{
    public class NServiceBusTriggerData
    {
        public byte[] Data { get; set; }
        public Dictionary<string, string> Headers { get; set; }
        public IMessageDispatcher Dispatcher { get; set; }
    }
}
