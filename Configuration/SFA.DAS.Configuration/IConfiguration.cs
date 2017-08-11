namespace SFA.DAS.Configuration
{
    public interface IConfiguration
    {
        string DatabaseConnectionString { get; set; }
        string ServiceBusConnectionString { get; set; }
    }
}
