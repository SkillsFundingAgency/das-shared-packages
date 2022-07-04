using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Triggers;
using NUnit.Framework;
using SFA.DAS.NServiceBus.AzureFunction.Hosting;

namespace SFA.DAS.NServiceBus.AzureFunction.UnitTests.Hosting
{
    public class WhenGettingTriggerBinderNServiceBusConnectionString
    {
        [Test]
        public async Task ThenReturnsTriggerBinding()
        {
            //Arrange
            var paramInfo = TestClass.GetParamInfoWithTriggerAttributeWithConnection();
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
            var paramInfo = TestClass.GetParamInfoWithoutTriggerAttribute();
            var context = new TriggerBindingProviderContext(paramInfo, new CancellationToken(false));
            var provider = new NServiceBusTriggerBindingProvider();

            //Act
            var result = await provider.TryCreateAsync(context);

            //Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task ThenFormatsAndPopulatesAttributeConnectionIfNull()
        {
            //Arrange
            const string nServiceBusConnectionString = "Endpoint=sb://new connection";
            Environment.SetEnvironmentVariable("NServiceBusConnectionString", nServiceBusConnectionString);
           
            var paramInfo = TestClass.GetParamInfoWithTriggerAttributeWithoutConnection();
            var context = new TriggerBindingProviderContext(paramInfo, new CancellationToken(false));
            var provider = new NServiceBusTriggerBindingProvider();

            //Act
            var result = await provider.TryCreateAsync(context);

            //Assert
            var binding = result as NServiceBusTriggerBinding;

            Assert.IsNotNull(binding);
#if NET6_0
            Assert.AreEqual("new connection", binding.Attribute.Connection);
#else
            Assert.AreEqual(nServiceBusConnectionString, binding.Attribute.Connection);
#endif
        }

        [Test]
        public async Task ThenDoesNotPopulateAttributeConnectionIfNotNull()
        {
            //Arrange
            var paramInfo = TestClass.GetParamInfoWithTriggerAttributeWithConnection();
            var context = new TriggerBindingProviderContext(paramInfo, new CancellationToken(false));
            var provider = new NServiceBusTriggerBindingProvider();

            //Act
            var result = await provider.TryCreateAsync(context);

            //Assert
            var binding = result as NServiceBusTriggerBinding;

            Assert.IsNotNull(binding);
            Assert.AreEqual(TestClass.ConnectionString, binding.Attribute.Connection);
        }
    }
}
