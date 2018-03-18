namespace Sfa.Automation.Framework.Enums
{
    /// <summary>
    /// List of supported WebDrivers that can be used.
    /// </summary>
    public enum WebDriver
    {
        /// <summary>
        /// Use Firefox Browser
        /// </summary>
        Firefox,
        /// <summary>
        /// Use Internet Explorer Browser
        /// </summary>
        InternetExplorer,
        /// <summary>
        /// Use Chrome Browser
        /// </summary>
        Chrome,
        /// <summary>
        /// Use Chrome Browser with Zap
        /// </summary>
        Zap,
        /// <summary>
        /// Use Browser stack to execute tests
        /// </summary>
        BrowserStack
    }
}
