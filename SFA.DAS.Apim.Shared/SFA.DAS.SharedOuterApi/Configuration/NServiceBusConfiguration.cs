namespace SFA.DAS.SharedOuterApi.Configuration
{
    public class NServiceBusConfiguration
    {
        public required string NServiceBusConnectionString { get; set; }
        public required string NServiceBusLicense { get; set; }
    }
}