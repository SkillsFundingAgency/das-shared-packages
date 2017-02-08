using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.Events.Api.Client.UnitTests.EventsApiAgreementsTests
{
    [TestFixture]
    public class WhenIGetAgreementEventsByDateRange : EventsApiTestBase
    {
        [Test]
        public async Task ThenAgreementEventsAreReturned()
        {
            var fromDate = DateTime.Now.AddDays(-30);
            var toDate = DateTime.Now;
            var pageSize = 500;
            var pageNumber = 3;

            var url = $"{BaseUrl}api/events/engagements?fromDate={fromDate:yyyyMMddHHmmss}&toDate={toDate:yyyyMMddHHmmss}&pageSize={pageSize}&pageNumber={pageNumber}";
            
            var expectedEvents = new List<AgreementEventView> { new AgreementEventView { CreatedOn = DateTime.Now.AddDays(-1), ContractType = "MainProvider", Event = "Test", Id = 87435, ProviderId = "ZZZ999" } };

            SecureHttpClient.Setup(x => x.GetAsync(url, ClientToken)).ReturnsAsync(JsonConvert.SerializeObject(expectedEvents));

            var response = await Api.GetAgreementEventsByDateRange(fromDate, toDate, pageSize, pageNumber);

            response.ShouldBeEquivalentTo(expectedEvents);
        }
    }
}
