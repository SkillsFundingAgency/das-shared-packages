using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;
using SFA.DAS.NServiceBus.AzureFunction.Hosting;

namespace SFA.DAS.NServiceBus.AzureFunction.UnitTests.Hosting
{
    public class WhenBindingTriggerValue
    {
        private NServiceBusTriggerBinding _binding;
        private ParameterInfo _parameterInfo;
        private NServiceBusTriggerAttribute _attribute;

        [SetUp]
        public void Arrange()
        {
            _parameterInfo = typeof(TestClass).GetMethod("TestMethod").GetParameters().First();
            _attribute = _parameterInfo.GetCustomAttribute<NServiceBusTriggerAttribute>();
            _binding = new NServiceBusTriggerBinding(_parameterInfo, _attribute);
        }

        [Test]
        public async Task ThenShouldReturnBinding()
        {
            //Arrange
            var testMessage = new TestMessage { Name = "Test" };
            var messageText = JsonConvert.SerializeObject(testMessage);
            var messageTextData = Encoding.UTF8.GetBytes(messageText);
            var triggerData = new NServiceBusTriggerData
            {
                Data = messageTextData
            };

            //Act
            var result = await _binding.BindAsync(triggerData, new ValueBindingContext(
                new FunctionBindingContext(Guid.NewGuid(), new CancellationToken()),
                new CancellationToken()));

            //Assert
            Assert.That(result, Is.Not.Null);

            var valueBinder = result.ValueProvider as NServiceBusMessageValueBinder;

            Assert.That(valueBinder, Is.Not.Null);

            var actualMessage = await valueBinder.GetValueAsync() as TestMessage;

            Assert.That(actualMessage, Is.Not.Null);
            Assert.That(actualMessage.Name, Is.EqualTo(testMessage.Name));
        }


        [Test]
        public void ThenThrowsExceptionIfValueIsNull()
        {
            //Act + Assert
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => _binding.BindAsync(null, new ValueBindingContext(
                new FunctionBindingContext(Guid.NewGuid(), new CancellationToken()),
                new CancellationToken())));

            Assert.That(exception.ParamName, Is.EqualTo("value"));
        }

        [Test]
        public void ThenThrowsExceptionIfValueNotSupported()
        {
            //Act + Assert
            var exception = Assert.ThrowsAsync<ArgumentException>(() => _binding.BindAsync("bad data", new ValueBindingContext(
                    new FunctionBindingContext(Guid.NewGuid(), new CancellationToken()),
                    new CancellationToken())));

            Assert.That(exception.ParamName, Is.EqualTo("value"));
        }

        [Test]
        public void ThenThrowsExceptionIfValueCannotBeDeseriaised()
        {
            //Arrange
            var triggerData = new NServiceBusTriggerData
            {
                Data = new byte[] { 1, 2, 3 }
            };

            //Act + Assert
            var exception = Assert.ThrowsAsync<ArgumentException>(() => _binding.BindAsync(triggerData, new ValueBindingContext(
                new FunctionBindingContext(Guid.NewGuid(), new CancellationToken()),
                new CancellationToken())));

            Assert.That(exception.ParamName, Is.EqualTo("value"));
        }

        private class TestMessage
        {
            public string Name { get; set; }
        }

        private static class TestClass
        {
            public static void TestMethod([NServiceBusTrigger] TestMessage message)
            {
                // Method intentionally left empty.
            }
        }
    }
}
