using System;
using NUnit.Framework;
using Sfa.Automation.Framework.Enums;
using Sfa.Automation.Framework.Selenium;


namespace Sfa.Automation.Framework.Test
{
    [TestFixture]
    public class HelloWorld : TestBase
    {
        private const string Url = "https://test-eas.apprenticeships.sfa.bis.gov.uk/";
        private const string SecurityUrl = "http://localhost:8080/json/core/view/alerts";
        private TestJson _testJson;

        [SetUp]
        public void SetUp()
        {
            _testJson = new TestJson();
        }

        [TestCase(WebDriver.Zap)]
        public void SimpleTest(WebDriver webDriver)
        {
            CommonTestSetup(new Uri(Url), true, webDriver);

            CommonTestSetup(new Uri(SecurityUrl), true, WebDriver.Chrome);

            var test = _testJson.DownloadSerializedJsonData<TestJson>(SecurityUrl);
        }

        [TearDown]
        public void TearDown()
        {
            WebBrowserDriver.Quit();
        }
    }
}