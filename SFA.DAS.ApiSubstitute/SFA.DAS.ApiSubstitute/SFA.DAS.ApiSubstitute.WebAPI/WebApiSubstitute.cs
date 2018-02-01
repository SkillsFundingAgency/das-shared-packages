using System;
using Microsoft.Owin.Hosting;
using SFA.DAS.ApiSubstitute.WebAPI.MessageHandlers;
using SFA.DAS.ApiSubstitute.WebAPI.OwinSelfHost;

namespace SFA.DAS.ApiSubstitute.WebAPI
{
    public class WebApiSubstitute : IDisposable
    {
        private DelegateHandler _apiMessageHandler;
        private IDisposable _webApi;

        public WebApiSubstitute(DelegateHandler apiMessageHandler)
        {
            _apiMessageHandler = apiMessageHandler;
            StartWebApiSubstitute();
        }

        private void StartWebApiSubstitute()
        {
            _webApi = WebApp.Start(_apiMessageHandler.BaseAddress, appBuilder => new WebApiStartUp().Configuration(appBuilder, _apiMessageHandler));
        }

        public void Dispose()
        {
            _webApi.Dispose();
        }
    }
}
