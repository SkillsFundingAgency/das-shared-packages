using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Bindings;
using Microsoft.Azure.WebJobs.Host.Listeners;
using Microsoft.Azure.WebJobs.Host.Protocols;
using Microsoft.Azure.WebJobs.Host.Triggers;
using Newtonsoft.Json;
using NServiceBus.Transport;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;

namespace SFA.DAS.NServiceBus.AzureFunction.Hosting
{
    public class NServiceBusTriggerBinding : ITriggerBinding
    {
        public ParameterInfo Parameter { get; }
        public NServiceBusTriggerAttribute Attribute { get; }

        private struct BindingNames
        {
            public const string Headers = "headers";
            public const string Dispatcher = "dispatcher";
        }

        public Type TriggerValueType => typeof(NServiceBusTriggerData);

        public IReadOnlyDictionary<string, Type> BindingDataContract => new Dictionary<string, Type>
        {
            {BindingNames.Headers, typeof(Dictionary<string, string>) },
            {BindingNames.Dispatcher, typeof(IDispatchMessages) }
        };

        public NServiceBusTriggerBinding(ParameterInfo parameter, NServiceBusTriggerAttribute attribute)
        {
            Parameter = parameter;
            Attribute = attribute;
        }

        public Task<ITriggerData> BindAsync(object value, ValueBindingContext context)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (!(value is NServiceBusTriggerData triggerData))
            {
                throw new ArgumentException($"Value must be of type {nameof(NServiceBusTriggerData)}", nameof(value));
            }

            object argument;

            try
            {
                var messageText = Encoding.UTF8.GetString(triggerData.Data);
                //Remove all extra invalid starting characters that have come from decoding bytes
                while (messageText.Length > 2 && messageText[0] != '{')
                {
                    messageText = messageText.Remove(0, 1);
                }

                if (Parameter.ParameterType == typeof(KeyValuePair<string,string>))
                {
                    argument = new KeyValuePair<string,string>(triggerData.Headers["NServiceBus.EnclosedMessageTypes"].Split(',')[0],messageText);
                }
                else
                {
                    argument = JsonConvert.DeserializeObject(messageText, Parameter.ParameterType);
                }

                
            }
            catch (Exception e)
            {
               throw new ArgumentException("Trigger data has invalid payload", nameof(value), e);
            }
            
            var valueBinder = new NServiceBusMessageValueBinder(Parameter, argument);

            var bindingData = new Dictionary<string, object>
            {
                {BindingNames.Headers, triggerData.Headers },
                {BindingNames.Dispatcher, triggerData.Dispatcher }
            };

            return Task.FromResult<ITriggerData>(new TriggerData(valueBinder, bindingData));
        }

        public Task<IListener> CreateListenerAsync(ListenerFactoryContext context)
        {
            return Task.FromResult<IListener>(new NServiceBusListener(context.Executor, Attribute, Parameter));
        }

        public ParameterDescriptor ToParameterDescriptor()
        {
            return new ParameterDescriptor
            {
                Name = Parameter.Name,
                DisplayHints = new ParameterDisplayHints
                {
                    Prompt = "NsbMessage",
                    Description = "NServiceBus trigger fired",
                    DefaultValue = "Sample"
                }
            };
        }
    }
}
