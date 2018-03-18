using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Specialized;
using System.Web.Script.Serialization;
using System.IO;
using System.Net;

namespace Sfa.Automation.Framework.Selenium
{
    public static class BrowserStackService
    {
        private static NameValueCollection Settings { get; set; }
        private static NameValueCollection Environments { get; set; }

        private static readonly string builddatetime;

        static BrowserStackService()
        {
            builddatetime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        }

        private static void Check()
        {
            if (Environments == null || Settings == null)
                throw new Exception("Invalid BrowserStack settings. Please, check available options in app.config or extend the last one.");
        }

        public static IWebDriver Init(string url)
        {
            Check();

            var capability = new DesiredCapabilities();

            /* Import settings from config file */
            foreach (var key in Environments.AllKeys)
                capability.SetCapability(key, Environments[key]);

            /* Set browserstack creds */
            capability.SetCapability("browserstack.user", Settings["username"]);
            capability.SetCapability("browserstack.key", Settings["key"]);

            /* Set browserstack.debug to true*/
            capability.SetCapability("browserstack.debug", "true");

            /* Set test name */
            capability.SetCapability("name", TestContext.CurrentContext.Test.Name);

            /* Set project name */
            capability.SetCapability("project", Settings["project"]);

            /* Set build name */
            capability.SetCapability("build", $"{Settings["build"]}-{ Settings["env"]}-{ builddatetime}");

            return new RemoteWebDriver(new Uri($"http://{Settings["server"]}/wd/hub/"), capability);
        }

        public static void MarkTestFailure(RemoteWebDriver driver,string exceptionMessage)
        {
            Check();

            var sessionId = driver.SessionId;

            var request = WebRequest.CreateHttp($"https://www.browserstack.com/automate/sessions/{sessionId}.json");
            request.ContentType = "application/json";
            request.Method = "PUT";
            request.Credentials = new NetworkCredential(Settings["username"], Settings["key"]);

            using (var stream = new StreamWriter(request.GetRequestStream()))
            {
                var content = new JavaScriptSerializer().Serialize(new
                {
                    status = "failed",
                    reason = exceptionMessage
                });
                stream.Write(content);
            }
            request.GetResponse();
        }
    }
}
