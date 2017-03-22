using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.Events.Api.Client.UnitTests.EventsApiApprenticeshipsTests
{
    [TestFixture]
    public class WhenBulkCreateApprenticeshipEvents : EventsApiTestBase
    {
        [Test]
        public async Task ThenApprenticeshipEventsAreCreated()
        {
            var url = $"{BaseUrl}api/events/apprenticeships/bulk";

            List<ApprenticeshipEvent> events = GetListOfTwoTestEvents();

            var expectedData = JsonConvert.SerializeObject(events);

            await Api.BulkCreateApprenticeshipEvent(events);

            SecureHttpClient.Verify(x => x.PostAsync(url, expectedData, ClientToken), Times.Once);
        }

        private static List<ApprenticeshipEvent> GetListOfTwoTestEvents()
        {
            return new List<ApprenticeshipEvent>
            {
                new ApprenticeshipEvent
                {
                    EmployerAccountId = "ABC123",
                    Event = "Test",
                    ProviderId = "ZZZ999",
                    AgreementStatus = AgreementStatus.BothAgreed,
                    ApprenticeshipId = 123,
                    LearnerId = "LID",
                    PaymentOrder = 1,
                    PaymentStatus = PaymentStatus.Completed,
                    TrainingEndDate = DateTime.Now.AddYears(3),
                    TrainingId = "ABC123",
                    TrainingStartDate = DateTime.Now.AddYears(-1),
                    TrainingTotalCost = 10000.34m,
                    TrainingType = TrainingTypes.Standard,
                    LegalEntityId = "LE ID 1",
                    LegalEntityName = "LE Name 1",
                    LegalEntityOrganisationType = "LE Org Type 1"
                },
                new ApprenticeshipEvent
                {
                    EmployerAccountId = "ABC123",
                    Event = "Test",
                    ProviderId = "ZZZ999",
                    AgreementStatus = AgreementStatus.BothAgreed,
                    ApprenticeshipId = 123,
                    LearnerId = "LID",
                    PaymentOrder = 1,
                    PaymentStatus = PaymentStatus.Completed,
                    TrainingEndDate = DateTime.Now.AddYears(3),
                    TrainingId = "ABC123",
                    TrainingStartDate = DateTime.Now.AddYears(-1),
                    TrainingTotalCost = 10000.34m,
                    TrainingType = TrainingTypes.Standard,
                    LegalEntityId = "LE ID 2",
                    LegalEntityName = "LE Name 2",
                    LegalEntityOrganisationType = "LE Org Type 2"
                }
            };
        }
    }
}
