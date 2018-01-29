using Microsoft.Owin.Hosting;
using SFA.DAS.ApiSubstitute.WebAPI.MessageHandlers;
using SFA.DAS.ApiSubstitute.WebAPI.OwinSelfHost;
using System;
using System.Collections.Generic;

namespace SFA.DAS.ApiSubstitute.WebAPI
{
    public class WebApiSubstitute : IDisposable
    {
        private string _baseAddress;
        private DelegateHandler _apiMessageHandler;
        private IDisposable _webApi;

        public WebApiSubstitute(string baseAddress, DelegateHandler apiMessageHandler)
        {
            _baseAddress = GetBaseUrl(baseAddress);
            _apiMessageHandler = apiMessageHandler;
            StartWebApiSubstitute();
        }

        public void StartWebApiSubstitute()
        {
            _webApi = WebApp.Start(_baseAddress, appBuilder => new WebApiStartUp().Configuration(appBuilder, _apiMessageHandler));
        }

        public void Dispose()
        {
            _webApi.Dispose();
        }

        private string GetBaseUrl(string baseAddress)
        {
            return baseAddress.EndsWith("/")
                ? baseAddress
                : baseAddress + "/";
        }
    }
}
