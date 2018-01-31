using System;
using System.Dynamic;
using System.Globalization;
using Newtonsoft.Json;
#if NET45
using Microsoft.Azure;
#else
using System.IO;
using Microsoft.Extensions.Configuration;
#endif

namespace SFA.DAS.NLog.Targets.Redis.DotNetCore
{
    // Class taken from https://github.com/ReactiveMarkets/NLog.Targets.ElasticSearch/commit/82bd41d46e15b08f3e7770e40f0660394f8359ba
    public static class StringExtensions
    {

#if !NET45
        private static IConfiguration _configuration;

        static  StringExtensions()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile("appsettings.Development.json", true, true);

            _configuration = builder.Build();
        }
#endif
        public static object ToSystemType(this string field, Type type, IFormatProvider formatProvider)
        {
            if (formatProvider == null)
            {
                formatProvider = CultureInfo.CurrentCulture;
            }

            switch (type.FullName)
            {
                case "System.Boolean":
                    return Convert.ToBoolean(field, formatProvider);
                case "System.Double":
                    return Convert.ToDouble(field, formatProvider);
                case "System.DateTime":
                    return Convert.ToDateTime(field, formatProvider);
                case "System.Int32":
                    return Convert.ToInt32(field, formatProvider);
                case "System.Int64":
                    return Convert.ToInt64(field, formatProvider);
                case "System.Object":
                    return JsonConvert.DeserializeObject<ExpandoObject>(field).ReplaceDotInKeys();
                default:
                    return field;
            }
        }

        public static string GetConfigurationValue(this string name)
        {
            var value = GetEnvironmentVariable(name);
            if (!string.IsNullOrEmpty(value))
                return value;

#if NET45
            var configValue = CloudConfigurationManager.GetSetting(name);
            return configValue;
#else
            return _configuration[name];
#endif
        }

        public static string GetConnectionString(this string name)
        {
            var value = GetEnvironmentVariable(name);
            if (!string.IsNullOrEmpty(value))
                return value;

#if NET45
            var connectionString = CloudConfigurationManager.GetSetting(name);
            return connectionString;
#else
            return _configuration.GetConnectionString(name);
#endif
        }

        private static string GetEnvironmentVariable(this string name)
        {
            return string.IsNullOrEmpty(name) ? null : Environment.GetEnvironmentVariable(name);
        }

    }
}