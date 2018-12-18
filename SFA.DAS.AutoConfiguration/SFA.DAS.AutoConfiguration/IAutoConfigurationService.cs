namespace SFA.DAS.AutoConfiguration
{
    public interface IAutoConfigurationService
    {
        T Get<T>();

        T Get<T>(string rowKey);
    }
}