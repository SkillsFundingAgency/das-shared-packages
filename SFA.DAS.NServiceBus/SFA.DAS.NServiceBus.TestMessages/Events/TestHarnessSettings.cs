namespace SFA.DAS.NServiceBus.TestMessages.Events
{
    public static class TestHarnessSettings
    {
        /// <summary>
        /// This should be same as `LearningTransportStorageDirectory` setting in
        /// SFA.DAS.NServiceBus.AzureFunctionExample/local.settings.json
        /// for a full end-to-end pub/sub test
        /// </summary>
        public const string LearningTransportDirectory = "c://temp//.sfa.das.nservicebus.learning-transport";

        public const string SampleQueueName = "SFA.DAS.NServiceBus.NetFrameworkEndpoint";
    }
}
