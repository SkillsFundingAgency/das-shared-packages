namespace SFA.DAS.AutoConfiguration
{
    public interface ITableStorageConfigurationService
    {
        T Get<T>();

        T Get<T>(string rowKey);
    }
}