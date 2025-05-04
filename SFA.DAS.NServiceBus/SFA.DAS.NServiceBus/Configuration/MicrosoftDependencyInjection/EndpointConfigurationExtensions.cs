//using NServiceBus;

//namespace SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection
//{
//    public static class EndpointConfigurationExtensions
//    {
//        public static EndpointConfiguration UseServicesBuilder(this EndpointConfiguration config, UpdateableServiceProvider serviceProvider)
//        {
//            config.UseContainer<ServicesBuilder>(c => c.ServiceProviderFactory(s => serviceProvider));

//            return config;
//        }
//    }
//}