using System;
using System.Threading;
using OpenQA.Selenium;
using Sfa.Automation.Framework.Constants;
using SeleniumExtras.PageObjects;
using OpenQA.Selenium.Support.UI;

namespace Sfa.Automation.Framework.Selenium
{
    /// <summary>
    /// Base Page that all Test Pages will Inherit to gain access to the IWebDriver.
    /// </summary>
    public class BasePage
    {
        /// <summary>
        /// IWebDriver Instance .
        /// </summary>
        public static IWebDriver Driver { get; set; }

        /// <summary>
        /// Initialise all the controls on the page so that they are accessible.
        /// </summary>
        /// <param name="webDriver">WebDriver that is in use.</param>
        protected BasePage(IWebDriver webDriver)
        {
            if (WaitForPageToFinishLoading(webDriver) == false)
            {
                throw new Exception("Timed out waiting for page to load.");
            }

            PageFactory.InitElements(webDriver, this);
        }

        /// <summary>
        /// Initialise all the controls on the page so that they are accessible.
        /// </summary>
        /// <param name="webDriver">WebDriver that is in use.</param>
        /// <param name="elementId">The element to initially check to ensure on correct page.</param>
        protected BasePage(IWebDriver webDriver, string elementId)
        {
            if (WaitForPageToFinishLoading(webDriver) == false)
            {
                throw new Exception("Timed out waiting for page to load.");
            }

            if (AssertElementIsDisplayed(elementId))
            {
                PageFactory.InitElements(webDriver, this);
            }
        }

        private bool WaitForPageToFinishLoading(IWebDriver webDriver)
        {
            Driver = webDriver;

            IWait<IWebDriver> wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(TimeoutInSeconds.DefaultTimeout));
            return wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));
        }

        private bool AssertElementIsDisplayed(string elementId)
        {
            const int upper = TimeoutInSeconds.DefaultTimeout;
            for (var i = 0; i < upper; i++)
            {
                try
                {
                    Driver.FindElement(By.Id(elementId));
                    return true;
                }
                catch (NoSuchElementException)
                {
                }
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            throw new Exception($"Could not find element with ID - {elementId} on page");
        }
    }
}
