namespace SFA.DAS.Client.Configuration
{
    public interface ITableStorageConfigurationService
    {
        T Get<T>();
    }
}