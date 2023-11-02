using NUnit.Framework;
using SFA.DAS.Telemetry.RedactionService;

namespace SFA.DAS.Telemetry.UnitTests
{
    public class RedactionServiceTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("http://www.google.com", "http://www.google.com")]
        [TestCase("http://www.google.com?colour=blue", "http://www.google.com?colour=blue")]
        [TestCase("http://www.google.com?email=chris@private.com", "http://www.google.com?email=REDACTED")]
        [TestCase("http://www.google.com?email=chris@private.com,john@alsohere.com", "http://www.google.com?email=REDACTED")]
        [TestCase("http://www.google.com?email=chris@private.com&isRobot=false", "http://www.google.com?email=REDACTED&isRobot=false")]
        [TestCase("http://www.google.com?isRobot=false&email=chris@private.com", "http://www.google.com?isRobot=false&email=REDACTED")]
        [TestCase("http://www.google.com?Email=chris@private.com", "http://www.google.com?Email=REDACTED")]
        [TestCase("http://www.google.com?Email=chris@private.com&dateofBirth=2019-11-10", "http://www.google.com?Email=REDACTED&dateofBirth=REDACTED")]
        public void TestRedaction(string originalUri, string expectedUri)
        {
            var uri = new Uri(originalUri);

            var options = new UriRedactionOptions
            {
                RedactionList = new List<string> { "email", "dateOfBirth" }
            };

            var service = new UriRedactionService(options);

            var result = service.GetRedactedUri(uri);
            Assert.That(result, Is.EqualTo(new Uri(expectedUri)));
        }
    }
}