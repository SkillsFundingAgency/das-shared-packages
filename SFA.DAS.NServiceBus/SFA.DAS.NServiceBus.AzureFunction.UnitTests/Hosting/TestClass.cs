using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;

namespace SFA.DAS.NServiceBus.AzureFunction.UnitTests.Hosting;

internal static class TestClass
{
    public const string ConnectionString = "test_Connection";
    public const string LearningTransportStorageDirectory = "test_LearningTransportStorageDirectory";

    public static ParameterInfo GetParamInfoWithTriggerAttributeWithoutConnection()
    {
        return GetParamsInfo(nameof(PlaceholderMethod)).First();
    }

    public static ParameterInfo GetParamInfoWithTriggerAttributeWithConnection()
    {
        return GetParamsInfo(nameof(PlaceholderMethod)).Skip(1).First();
    }

    public static ParameterInfo GetParamInfoWithTriggerAttributeWithoutLearningTransportStorageDirectory()
    {
        return GetParamsInfo(nameof(PlaceholderMethod)).First();
    }

    public static ParameterInfo GetParamInfoWithTriggerAttributeWithLearningTransportStorageDirectory()
    {
        return GetParamsInfo(nameof(PlaceholderMethod)).Skip(1).First();
    }

    public static ParameterInfo GetParamInfoWithoutTriggerAttribute()
    {
        return GetParamsInfo(nameof(PlaceholderMethod)).Last();
    }

    private static IEnumerable<ParameterInfo> GetParamsInfo(string methodName)
    {
        return typeof(TestClass).GetMethod(methodName)?.GetParameters();
    }

    //This must be public for reflection to work
    public static void PlaceholderMethod([NServiceBusTrigger] string trigger,
        [NServiceBusTrigger(Connection = ConnectionString, LearningTransportStorageDirectory = LearningTransportStorageDirectory)] string triggerWithConnection, string notATrigger)
    {
        // Method intentionally left empty.
    }
}