﻿using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.ApiSubstitute.WebAPI.Extensions;
using SFA.DAS.NLog.Logger;
using System;

namespace SFA.DAS.ApiSubstitute.WebAPI.MessageHandlers
{
    public class ApiMessageHandlers : DelegateHandler
    {
        private class Response
        {
            public HttpStatusCode _statusCode;
            public object _response;

            public Response(HttpStatusCode statusCode, object response)
            {
                _statusCode = statusCode;
                _response = response;
            }
        }

        private Dictionary<string, Response> _configuredGets = new Dictionary<string, Response>();

        private ILog _logger;

        public ApiMessageHandlers(string baseAddress, ILog logger) : base(baseAddress)
        {
            _logger = logger;
        }

        public ApiMessageHandlers(string baseAddress) : base(baseAddress)
        {
            _logger = new NLogLogger(typeof(ApiMessageHandlers), null, null);
        }

        public void SetupGet<T>(string endPoint, T response)
        {
            SetupGet(endPoint, HttpStatusCode.OK, response);
        }

        public void SetupGet<T>(string endPoint, HttpStatusCode httpStatusCode, T response)
        {
            var key = BaseAddress + endPoint.GetEndPoint();

            _configuredGets.Add(key, new Response(httpStatusCode, response));

            _logger.Info($"Configured get for {key}");
        }

        public void SetupPut(string endPoint)
        {
            var key = BaseAddress + endPoint.GetEndPoint();

            if (_configuredGets.ContainsKey(key))
            {
                _configuredGets.Remove(key);
            }
        }

        public void SetupCall<T>(string endPoint, HttpStatusCode httpStatusCode, T response)
        {
            SetupPut(endPoint);
            SetupGet(endPoint, httpStatusCode, response);
        }

        public void ClearSetup()
        {
            _configuredGets.Clear();
        }

        protected override Task<HttpResponseMessage> DoSendAsync(HttpRequestMessage request)
        {
            var requestUri = request.RequestUri.ToString();
            HttpResponseMessage response;
            if (!_configuredGets.ContainsKey(requestUri))
            {
                response = request.CreateResponse(HttpStatusCode.NotFound);
                _logger.Warn($"Response is not configured for {requestUri}, configured gets are {string.Join(Environment.NewLine, _configuredGets.Keys.ToString())}");
            }
            else
            {
                response = request.CreateResponse(_configuredGets[requestUri]._statusCode, _configuredGets[requestUri]._response);
                _logger.Info($"Response configured for {requestUri}");
            }

            var tsc = new TaskCompletionSource<HttpResponseMessage>();
            tsc.SetResult(response);
            return tsc.Task;
        }
    }
}
