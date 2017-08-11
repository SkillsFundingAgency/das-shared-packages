using System;
using System.Text;
using System.Web;
using System.Web.Security;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;

namespace SFA.DAS.CookieService.UnitTests.CookieTests
{
    public class WhenStoringInformationInTheCookie
    {
        private HttpCookieService<TestClass> _cookieService;
        private Mock<HttpContextBase> _httpContext;
        private Mock<HttpRequestBase> _httpRequest;
        private Mock<HttpResponseBase> _httpResponse;

        [SetUp]
        public void Arrange()
        {
            _cookieService = new HttpCookieService<TestClass>();

            _httpRequest = new Mock<HttpRequestBase>();
            _httpResponse = new Mock<HttpResponseBase>();
            _httpResponse.Setup(x => x.Cookies).Returns(new HttpCookieCollection());
            _httpRequest.Setup(x => x.Cookies).Returns(new HttpCookieCollection());

            _httpContext = new Mock<HttpContextBase>();
            _httpContext.Setup(x => x.Request).Returns(_httpRequest.Object);
            _httpContext.Setup(x => x.Response).Returns(_httpResponse.Object);
        }

        [Test]
        public void ThenTheCookieIsCreatedWithThePassedInName()
        {
            //Arrange
            var expectedCookieName = "TestCookie";

            //Act
            _cookieService.Create(_httpContext.Object, expectedCookieName, new TestClass(), 1);

            //Assert
            Assert.IsNotNull(_httpContext.Object.Response.Cookies);
            var testCookie = _httpContext.Object.Response.Cookies[expectedCookieName];
            Assert.IsNotNull(testCookie);
        }

        [Test]
        public void ThenTheCookieIsCreatedAndTheDataIsEncrypted()
        {
            //Arrange
            var expectedCookieName = "TestCookie";
            var data = new TestClass
            {
                TestName = "some value"
            };

            //Act
            _cookieService.Create(_httpContext.Object, expectedCookieName, data, 1);

            //Assert
            Assert.IsNotNull(_httpContext.Object.Response.Cookies);
            var testCookie = _httpContext.Object.Response.Cookies[expectedCookieName];
            Assert.IsNotNull(testCookie);
            var actualValue = Convert.FromBase64String(testCookie.Value);
            Assert.IsNotNull(actualValue);
            var actualObject = JsonConvert.DeserializeObject<TestClass>(Encoding.UTF8.GetString(MachineKey.Unprotect(actualValue)));
            Assert.IsNotNull(actualObject);
            Assert.AreEqual(data.TestName, actualObject.TestName);
        }

        [Test]
        public void ThenTheDataIsUpdatedInTheCookie()
        {
            //Arrange
            var expectedCookieName = "TestCookie";
            var valueUpdated = false;
            var data = new TestClass
            {
                TestName = "some value"
            };
            _httpResponse.Setup(x => x.SetCookie(It.IsAny<HttpCookie>())).Callback(() =>
            {
                valueUpdated = true;
            });
            _httpRequest.Object.Cookies.Add(new HttpCookie(expectedCookieName));
            _httpResponse.Object.Cookies.Add(new HttpCookie(expectedCookieName));
            
            //Act
            data.TestName = "Test2";
            _cookieService.Update(_httpContext.Object, expectedCookieName, data);

            //Assert
            Assert.IsTrue(valueUpdated);
        }

        [Test]
        public void ThenTheCookieExpiryIsSetInThePastWhenItIsExpired()
        {
            //Arrange
            var expectedCookieName = "TestCookie";
            var valueUpdated = false;
            _httpResponse.Setup(x => x.SetCookie(It.Is<HttpCookie>(c=>c.Expires <= DateTime.UtcNow))).Callback(() =>
            {
                valueUpdated = true;
            });
            _httpRequest.Object.Cookies.Add(new HttpCookie(expectedCookieName));
            _httpResponse.Object.Cookies.Add(new HttpCookie(expectedCookieName));

            //Act
            _cookieService.Delete(_httpContext.Object, expectedCookieName);

            //Assert
            Assert.IsTrue(valueUpdated);
        }

        [Test]
        public void ThenIfTheCookieDoesNotExistThenNullIsReturned()
        {
            //Act
            var actual = _cookieService.Get(_httpContext.Object, "test");

            //Assert
            Assert.IsNull(actual);
        }

        [Test]
        public void ThenIfTheCookieDoesExistTheObjectIsReturned()
        {
            //Arrange
            var expectedCookieName = "TestCookie";
            var data = new TestClass
            {
                TestName = "some value"
            };
            var content = Convert.ToBase64String(MachineKey.Protect(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data))));
            _httpRequest.Object.Cookies.Add(new HttpCookie(expectedCookieName, content));

            //Act
            var actual = _cookieService.Get(_httpContext.Object, expectedCookieName);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsAssignableFrom<TestClass>(actual);
        }

        internal class TestClass
        {
            public string TestName { get; set; }
        }
    }
}
