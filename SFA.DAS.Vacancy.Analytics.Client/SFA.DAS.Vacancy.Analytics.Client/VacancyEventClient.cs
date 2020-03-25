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
		private const string CustomEventDataTypeKey = "Type";
		private readonly string _eventHubSendConnectionString;
		private readonly string _publisherId;
		private readonly ILogger<VacancyEventClient> _logger;
		private readonly RetryPolicy _retryPolicy = new RetryExponential(TimeSpan.Zero, TimeSpan.FromSeconds(3), DefaultMaxRetrySendAttempts);
        private readonly EventHubClient _client;

		public VacancyEventClient(string eventHubSendConnectionString, string publisherId, ILogger<VacancyEventClient> logger, RetryPolicy retryPolicy = null)
		{
			_eventHubSendConnectionString = eventHubSendConnectionString;
            _publisherId = publisherId;
			_logger = logger;

			if (retryPolicy != null)
				_retryPolicy = retryPolicy;

            _client = EventHubClient.CreateFromConnectionString(_eventHubSendConnectionString);
            _client.RetryPolicy = _retryPolicy;
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
			evtData.Properties.Add(CustomEventDataTypeKey, evt.EventType);

			try
			{
				await _client.SendAsync(evtData);
				await _client.CloseAsync();
			}
			catch (EventHubsException ex)
			{
				_logger.LogError($"Error publishing event: {evt.EventType}.", ex);
			}
		}

		public async Task PublishBatchEventsAsync<T>(IEnumerable<T> events) where T : VacancyEvent
		{
			if (events.Count() < 1)
			{
				throw new ArgumentException("Must supply at least one event to publish as part of batch");
			}

			var exMsg = $"Error publishing batch events: {typeof(T).Name}.";

			try
			{
				var batchedEvents = _client.CreateBatch();

				var addedSuccesfully = PopulateBatch(events, batchedEvents);

				if (!addedSuccesfully) throw new BatchEventPublishException(exMsg);

				await _client.SendAsync(batchedEvents);
				await _client.CloseAsync();
			}
			catch (EventHubsException ex)
			{
				_logger.LogError(exMsg, ex);
			}
		}

		private bool PopulateBatch<T>(IEnumerable<T> events, EventDataBatch batchedEvents) where T : VacancyEvent
		{
			_logger.LogDebug($"Attempting to load {events.Count()} events of type {typeof(T).Name} for batch publish.");

			return events.All(evt =>
			{
				evt.PublisherId = _publisherId;
				var evtData = new EventData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(evt)));
				evtData.Properties.Add(CustomEventDataTypeKey, evt.EventType);
				var hasAdded = batchedEvents.TryAdd(evtData);
				return hasAdded;
			});
		}
	}
}