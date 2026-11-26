using System;
using NServiceBus.Raw;

namespace SFA.DAS.NServiceBus.AzureFunction.Configuration
{
    public static class RawEndpointConfigurationExtension
    {
        public static void UseLicense(this RawEndpointConfiguration config, string licenseText)
        {
            if (string.IsNullOrWhiteSpace(licenseText))
            {
                throw new ArgumentException("NServiceBus license text must not be null or white space", nameof(licenseText));
            }

            // TODO: Set license text?
            //config.Settings.Set("LicenseText", licenseText);
        }
    }
}
