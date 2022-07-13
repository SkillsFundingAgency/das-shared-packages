using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Triggers;
using NUnit.Framework;
using SFA.DAS.NServiceBus.AzureFunction.Hosting;

namespace SFA.DAS.NServiceBus.AzureFunction.UnitTests.Hosting
{
    public class WhenGettingTriggerBinderLearningTransportStorageDirectory
    {
        [Test]
        public async Task ThenPopulatesLearningTransportStorageDirectoryIfNull()
        {
            //Arrange
            const string learningTransportStorageDirectory = "C://temp//.learning-transport";
            Environment.SetEnvironmentVariable("LearningTransportStorageDirectory", learningTransportStorageDirectory, EnvironmentVariableTarget.Process);
            const string nServiceBusConnectionString = "Endpoint=sb://new connection";
            Environment.SetEnvironmentVariable("NServiceBusConnectionString", nServiceBusConnectionString);

            var paramInfo = TestClass.GetParamInfoWithTriggerAttributeWithoutLearningTransportStorageDirectory();
            var context = new TriggerBindingProviderContext(paramInfo, new CancellationToken(false));
            var provider = new NServiceBusTriggerBindingProvider();

            //Act
            var result = await provider.TryCreateAsync(context);

            //Assert
            var binding = result as NServiceBusTriggerBinding;

            Assert.IsNotNull(binding);
            Assert.AreEqual(learningTransportStorageDirectory, binding.Attribute.LearningTransportStorageDirectory);
        }

        [Test]
        public async Task ThenDoesNotPopulateLearningTransportStorageDirectoryIfNotNull()
        {
            //Arrange
            var paramInfo = TestClass.GetParamInfoWithTriggerAttributeWithLearningTransportStorageDirectory();
            var context = new TriggerBindingProviderContext(paramInfo, new CancellationToken(false));
            var provider = new NServiceBusTriggerBindingProvider();

            //Act
            var result = await provider.TryCreateAsync(context);

            //Assert
            var binding = result as NServiceBusTriggerBinding;

            Assert.IsNotNull(binding);
            Assert.AreEqual(TestClass.LearningTransportStorageDirectory, binding.Attribute.LearningTransportStorageDirectory);
        }
    }
}
