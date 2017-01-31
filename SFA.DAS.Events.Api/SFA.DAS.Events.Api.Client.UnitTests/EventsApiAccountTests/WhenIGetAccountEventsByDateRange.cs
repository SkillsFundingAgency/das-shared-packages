using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.Events.Api.Client.UnitTests.EventsApiAccountTests
{
    [TestFixture]
    public class WhenIGetAccountEventsByDateRange : EventsApiTestBase
    {
        [Test]
        public async Task ThenAccountEventsAreReturned()
        {
            var fromDate = DateTime.Now.AddDays(-30);
            var toDate = DateTime.Now;
            var pageSize = 500;
            var pageNumber = 3;

            var url = $"{BaseUrl}api/events/accounts?fromDate={fromDate:yyyyMMddHHmmss}&toDate={toDate:yyyyMMddHHmmss}&pageSize={pageSize}&pageNumber={pageNumber}";
            
            var expectedEvents = new List<AccountEventView> { new AccountEventView { CreatedOn = DateTime.Now.AddDays(-1), ResourceUri = "/api/accounts/ABC123", Event = "Test", Id = 87435 } };

            SecureHttpClient.Setup(x => x.GetAsync(url, ClientToken)).ReturnsAsync(JsonConvert.SerializeObject(expectedEvents));

            var response = await Api.GetAccountEventsByDateRange(fromDate, toDate, pageSize, pageNumber);

            response.ShouldBeEquivalentTo(expectedEvents);
        }
    }
}
