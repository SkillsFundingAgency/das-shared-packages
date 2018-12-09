using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Esfa.Vacancy.Analytics.Events;
using System.Collections.Generic;

namespace Esfa.Vacancy.Analytics
{
	public class VacancyEventClient : IVacancyEventClient
	{
		private const int DefaultMaxRetrySendAttempts = 3;
		private readonly string _eventHubSendConnectionString;
		private readonly string _publisherId;
		private readonly ILogger<VacancyEventClient> _logger;
		private readonly RetryPolicy _retryPolicy = new RetryExponential(TimeSpan.Zero, TimeSpan.FromSeconds(3), DefaultMaxRetrySendAttempts);

		public VacancyEventClient(string eventHubSendConnectionString, string publisherId, ILogger<VacancyEventClient> logger, RetryPolicy retryPolicy = null)
		{
			_eventHubSendConnectionString = eventHubSendConnectionString;
			_publisherId = publisherId;
			_logger = logger;

			if (retryPolicy != null)
				_retryPolicy = retryPolicy;
		}

		public async Task PushApprenticeshipSearchEventAsync(long vacancyReference)
		{
			var evt = new ApprenticeshipSearchImpressionEvent(vacancyReference, _publisherId);
			await PublishEventAsync(evt);
		}

		public async Task PushApprenticeshipDetailEventAsync(long vacancyReference)
		{
			var evt = new ApprenticeshipDetailImpressionEvent(vacancyReference, _publisherId);
			await PublishEventAsync(evt);
		}

		public async Task PushApprenticeshipBookmarkedEventAsync(long vacancyReference)
		{
			var evt = new ApprenticeshipBookmarkedImpressionEvent(vacancyReference, _publisherId);
			await PublishEventAsync(evt);
		}

		public async Task PushApprenticeshipSavedSearchAlertEventAsync(long vacancyReference)
		{
			var evt = new ApprenticeshipSavedSearchAlertImpressionEvent(vacancyReference, _publisherId);
			await PublishEventAsync(evt);
		}

		public async Task PushApprenticeshipApplicationCreatedEventAsync(long vacancyReference)
		{
			var evt = new ApprenticeshipApplicationCreatedEvent(vacancyReference, _publisherId);
			await PublishEventAsync(evt);
		}

		public async Task PushApprenticeshipApplicationSubmittedEventAsync(long vacancyReference)
		{
			var evt = new ApprenticeshipApplicationSubmittedEvent(vacancyReference, _publisherId);
			await PublishEventAsync(evt);
		}

		private async Task PublishEventAsync(VacancyEvent evt)
		{
			var evtData = new EventData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(evt)));

			try
			{
				var client = GetClient();
				await client.SendAsync(evtData);
				await client.CloseAsync();
			}
			catch (EventHubsException ex)
			{
				_logger.LogError($"Error publishing event: {evt.EventType}.", ex);
			}
		}

		public async Task PublishBatchEventsAsync<T>(IEnumerable<T> events) where T : VacancyEvent
		{
			var exMsg = $"Error publishing batch events: {nameof(T)}.";

			try
			{
				var client = GetClient();
				var batchedEvents = client.CreateBatch(new BatchOptions { MaxMessageSize = events.Count() });

				var addedSuccesfully = PopulateBatch(events, batchedEvents);

				if (!addedSuccesfully) throw new BatchEventPublishException(exMsg);

				await client.SendAsync(batchedEvents);
				await client.CloseAsync();
			}
			catch (EventHubsException ex)
			{
				_logger.LogError(exMsg, ex);
			}
		}

		private bool PopulateBatch<T>(IEnumerable<T> events, EventDataBatch batchedEvents) where T : VacancyEvent
		{
			return events.All(evt =>
			{
				evt.PublisherId = _publisherId;
				var evtData = new EventData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(evt)));
				return batchedEvents.TryAdd(evtData);
			});
		}

		private EventHubClient GetClient()
		{
			var client = EventHubClient.CreateFromConnectionString(_eventHubSendConnectionString);
			client.RetryPolicy = _retryPolicy;
			return client;
		}
	}
}