using Owin;
using System.Web.Http;
using SFA.DAS.ApiSubstitute.WebAPI.MessageHandlers;

namespace SFA.DAS.ApiSubstitute.WebAPI.OwinSelfHost
{
    public class WebApiStartUp
    {
        // This code configures Web API to use the Delegate message handlers. 
        // The WebApiStartUp class is specified as a type parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder, DelegateHandler messageHandlers)
        {
            HttpConfiguration config = new HttpConfiguration();

            //Add message Handlers to the Http Config
            config.MessageHandlers.Add(messageHandlers);

            //Request the App Builder to use the config
            appBuilder.UseWebApi(config);
        }
    }
}
