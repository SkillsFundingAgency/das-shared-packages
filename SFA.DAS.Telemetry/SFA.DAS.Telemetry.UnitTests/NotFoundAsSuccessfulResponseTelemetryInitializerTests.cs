using Microsoft.ApplicationInsights.DataContracts;
using NUnit.Framework;
using SFA.DAS.Telemetry.Telemetry;

namespace SFA.DAS.Telemetry.UnitTests
{
    public class NotFoundAsSuccessfulResponseTelemetryInitializerTests
    {
        [Test]
        public void NotFoundResponseIsLoggedAsSuccess()
        {
            var sut = new NotFoundAsSuccessfulResponseTelemetryInitializer();

            var telemetry = new RequestTelemetry
            {
                ResponseCode = "404",
                Success = false
            };

            sut.Initialize(telemetry);

            Assert.That(telemetry.Success, Is.True);
        }
    }
}
