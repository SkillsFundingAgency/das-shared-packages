

using Microsoft.Azure;
using Microsoft.WindowsAzure;

namespace SFA.DAS.Configuration.AzureTableStorage
{
    public class CloudConfigurationSettings
    {
        public string GetSetting(string setting)
        {
            return CloudConfigurationManager.GetSetting(setting);
        }
    }
}
