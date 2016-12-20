using System;
using OpenQA.Selenium;


namespace Sfa.Automation.Framework.Selenium
{
    /// <summary>
    /// Class to set WebDriver timeouts
    /// </summary>
    public static class SetTimeout
    {
        /// <summary>
        /// Set the Implicit Wait Timeout.
        /// </summary>
        /// <param name="driver">WebDriver Instance</param>
        /// <param name="timeSpan">Timespan to wait for</param>
        public static void ImplicityWait(IWebDriver driver, TimeSpan timeSpan)
        {
            driver.Manage().Timeouts().ImplicitlyWait(timeSpan);
        }

        /// <summary>
        /// Set the Page Load Timeout.
        /// </summary>
        /// <param name="driver">WebDriver Instance</param>
        /// <param name="timeSpan">Timespan to wait for</param>
        public static void PageLoad(IWebDriver driver, TimeSpan timeSpan)
        {
            driver.Manage().Timeouts().SetPageLoadTimeout(timeSpan);
        }

        /// <summary>
        /// Set the Script Timeout.
        /// </summary>
        /// <param name="driver">WebDriver Instance</param>
        /// <param name="timeSpan">Timespan to wait for</param>
        public static void Script(IWebDriver driver, TimeSpan timeSpan)
        {
            driver.Manage().Timeouts().SetScriptTimeout(timeSpan);
        }

    }
}
