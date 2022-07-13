using System;
using System.Security.Cryptography;
using System.Text;

namespace SFA.DAS.NServiceBus.Configuration.AzureServiceBus
{
    public static class RuleNameShortener
    {
        private const int AzureServiceBusRuleNameMaxLength = 50;
        
        public static string Shorten(Type arg)
        {
            var ruleName = arg.FullName;
            if (ruleName!.Length <= AzureServiceBusRuleNameMaxLength)
            {
                return ruleName;
            }

            using var md5 = MD5.Create();
            var bytes = Encoding.Default.GetBytes(ruleName);
            var hash = md5.ComputeHash(bytes);
            var shortenedRuleName = new Guid(hash).ToString();

            return shortenedRuleName;
        }
    }
}