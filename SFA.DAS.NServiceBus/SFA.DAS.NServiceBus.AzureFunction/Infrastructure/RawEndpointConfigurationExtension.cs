using System;
using NServiceBus.Raw;

namespace SFA.DAS.NServiceBus.AzureFunction.Infrastructure
{
    public static class RawEndpointConfigurationExtension
    {
        public static void License(this RawEndpointConfiguration config, string licenseText)
        {
            if (string.IsNullOrEmpty(licenseText))
            {
                throw new ArgumentException("NServiceBus license text much not be empty", nameof(licenseText));
            }

            config.Settings.Set("LicenseText", (object) licenseText);
        }
    }
}
