using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Triggers;
using NUnit.Framework;
using SFA.DAS.NServiceBus.AzureFunction.Infrastructure;

namespace SFA.DAS.Reservations.Infrastructure.UnitTests.NServiceBus
{
    public class WhenGettingTriggerBinder
    {
        [Test]
        public async Task ThenReturnsTriggerBinding()
        {
            //Arrange
            var paramInfo = TestClass.GetParamInfoWithTriggerAttrubuteWithConnection();
            var context = new TriggerBindingProviderContext(paramInfo, new CancellationToken(false));
            var provider = new NServiceBusTriggerBindingProvider();

            //Act
            var result = await provider.TryCreateAsync(context);

            //Assert
            var binding = result as NServiceBusTriggerBinding;

            Assert.IsNotNull(binding);
        }

        [Test]
        public async Task ThenReturnsNullIfNoAttributeFound()
        {
            //Arrange
            var paramInfo = TestClass.GetParamInfoWithoutTriggerAttrubute();
            var context = new TriggerBindingProviderContext(paramInfo, new CancellationToken(false));
            var provider = new NServiceBusTriggerBindingProvider();

            //Act
            var result = await provider.TryCreateAsync(context);

            //Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task ThenPopulatesAttributeConnectionIfNull()
        {
            //Arrange
            var nServiceBusConnectionString = "new connection";
            Environment.SetEnvironmentVariable("NServiceBusConnectionString", nServiceBusConnectionString);
           
            
            var paramInfo = TestClass.GetParamInfoWithTriggerAttrubuteWithoutConnection();
            var context = new TriggerBindingProviderContext(paramInfo, new CancellationToken(false));
            var provider = new NServiceBusTriggerBindingProvider();

            //Act
            var result = await provider.TryCreateAsync(context);

            //Assert
            var binding = result as NServiceBusTriggerBinding;

            Assert.IsNotNull(binding);
            Assert.AreEqual(nServiceBusConnectionString, binding.Attribute.Connection);
        }

        [Test]
        public async Task ThenDoesNotPopulatesAttributeConnectionIfPopulated()
        {
            //Arrange
            var paramInfo = TestClass.GetParamInfoWithTriggerAttrubuteWithConnection();
            var context = new TriggerBindingProviderContext(paramInfo, new CancellationToken(false));
            var provider = new NServiceBusTriggerBindingProvider();

            //Act
            var result = await provider.TryCreateAsync(context);

            //Assert
            var binding = result as NServiceBusTriggerBinding;

            Assert.IsNotNull(binding);
            Assert.AreEqual(TestClass.ConnectionString, binding.Attribute.Connection);
        }

        private class TestClass
        {
            public const string ConnectionString = "test_Connection";

            public static ParameterInfo GetParamInfoWithTriggerAttrubuteWithoutConnection()
            {
                return GetParamsInfo(nameof(PlaceholderMethod)).First();
            }

            public static ParameterInfo GetParamInfoWithTriggerAttrubuteWithConnection()
            {
                return GetParamsInfo(nameof(PlaceholderMethod)).Skip(1).First();
            }

            public static ParameterInfo GetParamInfoWithoutTriggerAttrubute()
            {
                return GetParamsInfo(nameof(PlaceholderMethod)).Last();
            }

            private static IEnumerable<ParameterInfo> GetParamsInfo(string methodName)
            {
                return typeof(TestClass).GetMethod(methodName).GetParameters();
            }

            //This must be public for reflection to work
            public static void PlaceholderMethod([NServiceBusTrigger]string trigger, [NServiceBusTrigger(Connection = ConnectionString)] string triggerWithConnection, string notATrigger)
            {

            }
        }
    }
}
