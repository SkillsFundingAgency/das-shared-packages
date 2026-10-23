using NUnit.Framework;
using SFA.DAS.Telemetry.RedactionService;

namespace SFA.DAS.Telemetry.UnitTests
{
    public class RedactionServiceTests
    {
        private UriRedactionService _sut;

        [SetUp]
        public void Setup()
        {
            var options = new UriRedactionOptions
            {
                RedactionList = new List<string>
                {
                    "email",
                    "dateOfBirth"
                }
            };

            _sut = new UriRedactionService(options);
        }

        [TestCase(
            "http://www.google.com",
            "http://www.google.com")]
        [TestCase(
            "http://www.google.com?*",
            "http://www.google.com?*")]
        [TestCase(
            "http://www.google.com?colour=blue",
            "http://www.google.com?colour=blue")]
        [TestCase(
            "http://www.google.com?email=chris@private.com",
            "http://www.google.com?email=REDACTED")]
        [TestCase(
            "http://www.google.com?email=chris@private.com,john@alsohere.com",
            "http://www.google.com?email=REDACTED")]
        [TestCase(
            "http://www.google.com?email=chris@private.com&isRobot=false",
            "http://www.google.com?email=REDACTED&isRobot=false")]
        [TestCase(
            "http://www.google.com?isRobot=false&email=chris@private.com",
            "http://www.google.com?isRobot=false&email=REDACTED")]
        [TestCase(
            "http://www.google.com?Email=chris@private.com",
            "http://www.google.com?Email=REDACTED")]
        [TestCase(
            "http://www.google.com?Email=chris@private.com&dateofBirth=2019-11-10",
            "http://www.google.com?Email=REDACTED&dateofBirth=REDACTED")]
        [TestCase(
            "http://www.google.com?unkeyed&email=chris@private.com",
            "http://www.google.com?unkeyed&email=REDACTED")]
        public void GetRedactedUri_ReturnsExpectedUri(
            string originalUri,
            string expectedUri)
        {
            var result = _sut.GetRedactedUri(new Uri(originalUri));

            Assert.That(result, Is.EqualTo(new Uri(expectedUri)));
        }

        [Test]
        public void GetRedactedUri_IgnoresInvalidRedactionKeys()
        {
            var options = new UriRedactionOptions
            {
                RedactionList = new List<string>
                {
                    null!,
                    string.Empty,
                    " ",
                    "email"
                }
            };

            _sut = new UriRedactionService(options);

            var result = _sut.GetRedactedUri(
                new Uri("http://www.google.com?email=chris@private.com"));

            Assert.That(
                result,
                Is.EqualTo(new Uri("http://www.google.com?email=REDACTED")));
        }

        [TestCase(
            "Request started http://www.google.com?email=chris@private.com&isRobot=false",
            "Request started http://www.google.com?email=REDACTED&isRobot=false")]
        [TestCase(
            "Request started http://www.google.com?EMAIL=chris@private.com&isRobot=false",
            "Request started http://www.google.com?EMAIL=REDACTED&isRobot=false")]
        public void GetRedactedString_ReturnsExpectedString(
            string originalString,
            string expectedString)
        {
            var result = _sut.GetRedactedString(originalString);
            Assert.That(result, Is.EqualTo(expectedString));
        }
    }
}