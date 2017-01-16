using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Events.Api.Client.UnitTests.Builders;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.Events.Api.Client.UnitTests.EventsApiApprenticeshipsTests
{
    [TestFixture]
    public class WhenIGetApprenticeshipEventsByDateRange : EventsApiTestBase
    {
        [Test]
        public async Task ThenApprenticeshipEventsAreReturned()
        {
            var fromDate = DateTime.Now.AddDays(-30);
            var toDate = DateTime.Now;
            var pageSize = 500;
            var pageNumber = 3;

            var url = $"{BaseUrl}api/events/apprenticeships?fromDate={fromDate:yyyyMMddHHmmss}&toDate={toDate:yyyyMMddHHmmss}&pageSize={pageSize}&pageNumber={pageNumber}";

            var expectedEvents = new List<ApprenticeshipEventView> { new ApprenticeshipEventViewBuilder().Build() };

            SecureHttpClient.Setup(x => x.GetAsync(url, ClientToken)).ReturnsAsync(JsonConvert.SerializeObject(expectedEvents));

            var response = await Api.GetApprenticeshipEventsByDateRange(fromDate, toDate, pageSize, pageNumber);

            response.ShouldBeEquivalentTo(expectedEvents);
        }
    }
}
