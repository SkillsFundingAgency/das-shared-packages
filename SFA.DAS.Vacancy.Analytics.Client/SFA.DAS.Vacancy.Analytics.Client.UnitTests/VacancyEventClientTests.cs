using System;
using Xunit;
using Moq;
using FluentAssertions;
using Esfa.Vacancy.Analytics;
using Esfa.Vacancy.Analytics.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.Vacancy.Analytics.Client.UnitTests
{
	public class VacancyEventClientTests
	{
		private const long ExampleVacancyReference = 1000000004;
		private const string TestPublisherId = "TEST";
		private readonly IVacancyEventClient _client;
		private readonly string TestEventHubEndpointAddress = "Endpoint=sb://vacancy-events.servicebus.windows.net/;SharedAccessKeyName=IngestVacancyEvents;SharedAccessKey=NeedsToBeReal;EntityPath=vacancy";

		public VacancyEventClientTests()
		{
			_client = new VacancyEventClient(TestEventHubEndpointAddress, TestPublisherId, Mock.Of<ILogger<VacancyEventClient>>());
		}

		[Fact(Skip = "Test requires a valid Azure EventHub namespace which is not committed with this test.")]
		public void GivenAnEventToPublishWhenClientBatchPublishCalledThenPublishCallSucceeds()
		{
			var evt = new ApprenticeshipApplicationSubmittedEvent(ExampleVacancyReference);

			_client.PublishBatchEventsAsync(new ApprenticeshipApplicationSubmittedEvent[] { evt }).GetAwaiter().GetResult();

			Assert.True(true);
		}

		[Fact(Skip = "Test requires a valid Azure EventHub namespace which is not committed with this test.")]
		public void GivenMultipleEventsToPublishWhenClientBatchPublishCalledThenPublishCallSucceeds()
		{
			var evt = new ApprenticeshipApplicationSubmittedEvent(ExampleVacancyReference);

			_client.PublishBatchEventsAsync(new ApprenticeshipApplicationSubmittedEvent[] { evt, evt }).GetAwaiter().GetResult();

			Assert.True(true);
		}

		[Fact]
		public void GivenNoEventsToPublishWhenClientBatchPublishCalledThenArgumentExceptionIsThrown()
		{
			Assert.Throws<ArgumentException>(() => _client.PublishBatchEventsAsync(new ApprenticeshipApplicationSubmittedEvent[] { }).GetAwaiter().GetResult());
		}
	}
}