using SFA.DAS.ApiSubstitute.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.HmrcApiSubstitute.WebAPI
{
    public class HmrcApi : WebApiSubstitute
    {
        public string BaseAddress { get; private set; }

        public HmrcApiMessageHandler ProviderEventsApiMessageHandler { get; private set; }

        public HmrcApi(HmrcApiMessageHandler apiMessageHandler) : base(apiMessageHandler)
        {
            BaseAddress = apiMessageHandler.BaseAddress;
            ProviderEventsApiMessageHandler = apiMessageHandler;
        }


    }
}
