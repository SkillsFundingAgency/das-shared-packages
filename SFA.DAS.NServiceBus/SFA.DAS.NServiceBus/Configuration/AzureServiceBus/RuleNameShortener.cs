using System;
using System.Security.Cryptography;
using System.Text;

namespace SFA.DAS.NServiceBus.Configuration.AzureServiceBus
{
    public class RuleNameShortener
    {
        private const int AzureServiceBusRuleNameMaxLength = 50;
        
        public string Shorten(string ruleName)
        {
            if (ruleName.Length <= AzureServiceBusRuleNameMaxLength)
            {
                return ruleName;
            }
            
            using (var md5 = MD5.Create())
            {
                var bytes = Encoding.Default.GetBytes(ruleName);
                var hash = md5.ComputeHash(bytes);
                var shortenedRuleName = new Guid(hash).ToString();

                return shortenedRuleName;
            }
        }
    }
}