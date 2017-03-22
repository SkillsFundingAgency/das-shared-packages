using System;
using System.Threading.Tasks;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.Events.Api.Client.UnitTests.EventsApiApprenticeshipsTests
{
    [TestFixture]
    public class WhenICreateAnApprenticeshipEvent : EventsApiTestBase
    {
        [Test]
        public async Task ThenApprenticeshipEventIsCreated()
        {
            var url = $"{BaseUrl}api/events/apprenticeships";

            var @event = new ApprenticeshipEvent
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
                LegalEntityId = "LE ID",
                LegalEntityName = "LE Name",
                LegalEntityOrganisationType = "LE Org Type"
            };
            var expectedData = JsonConvert.SerializeObject(@event);

            await Api.CreateApprenticeshipEvent(@event);

            SecureHttpClient.Verify(x => x.PostAsync(url, expectedData, ClientToken), Times.Once);
        }
    }
}
