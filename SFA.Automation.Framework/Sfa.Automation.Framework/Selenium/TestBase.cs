using Sfa.Automation.Framework.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using Sfa.Automation.Framework.Constants;
using OpenQA.Selenium.Chrome;
using Sfa.Automation.Framework.AzureUtils;


namespace Sfa.Automation.Framework.Selenium
{
    /// <summary>
    /// Base class used for all tests to inherit.
    /// </summary>
    public abstract class TestBase
    {
        /// <summary>
        /// Instance of the WebDriver used during the tests.
        /// </summary>
        protected IWebDriver WebBrowserDriver { get; set; }

        /// <summary>
        /// Start up the WebDriver and navigate to the URL specified.
        /// </summary>
        /// <param name="url">The URL that will be loaded in the web page.</param>
        /// <param name="deleteAllCookies">Should the cookies be deleted before starting the browser.</param>
        /// <param name="webDriver">The webDriver that will be used during the test.</param>
        /// 
        protected void CommonTestSetup(Uri url, bool deleteAllCookies = true, WebDriver webDriver = WebDriver.Firefox)
        {
            switch (webDriver)
            {
                case WebDriver.Firefox:
                    InitialiseFirefox(url, deleteAllCookies);
                    break;
                case WebDriver.InternetExplorer:
                    InitialiseInternetExplorer(url, deleteAllCookies);
                    break;
                case WebDriver.Chrome:
                    InitialiseChrome(url, deleteAllCookies);
                    break;
                case WebDriver.Zap:
                    InitialiseZapChrome(url, deleteAllCookies);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(webDriver), webDriver, null);
            }
        }

        /// <summary>
        /// Delete and then create all Azure Table Storage tables that are not empty.
        /// </summary>
        /// <param name="connectionString">The connection string to the Azure Storage.</param>
        /// <param name="tablesToExclude">The tables in storage to not recreate when deleting all tables.</param>
        protected static void InitialiseTableStorage(string connectionString, List<string> tablesToExclude)
        {
            var tablesStorage = new AzureTableStorage(connectionString, tablesToExclude);
            var tables = tablesStorage.GetAllTableNamesForAzureAccount(tablesToExclude.Count() != 0);
            var populatedTables = (from table in tables let manager = new AzureTableManager(connectionString, table.Name) where manager.GetNumberOfEntities() != 0 select table).ToList();

            foreach (var tableManager in populatedTables.Select(cloudTable => new AzureTableManager(connectionString, cloudTable.Name)))
            {
                tableManager.DeleteTable();
            }

            foreach (var tableManager in populatedTables.Select(cloudTable => new AzureTableManager(connectionString, cloudTable.Name)))
            {
                tableManager.CreateTable();
            }
        }

        private void InitialiseFirefox(Uri url, bool deleteAllCookies)
        {
            var firefoxOptions = new FirefoxOptions();

            firefoxOptions.SetPreference("browser.download.folderList", 2);
            firefoxOptions.SetPreference("browser.download.dir", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads\\");
            firefoxOptions.SetPreference("browser.helperApps.neverAsk.saveToDisk", "application/zip");

            const int maxAttempts = 3;
            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                var message = string.Empty;
                try
                {
                    //The geckodriver will need to be placed here. Update to ensure that this is not required.
                    var driverService = FirefoxDriverService.CreateDefaultService(@"C:\Selenium", "geckodriver.exe");
                    driverService.FirefoxBinaryPath = @"C:\Program Files\Mozilla Firefox\firefox.exe";
                    driverService.HideCommandPromptWindow = true;
                    driverService.SuppressInitialDiagnosticInformation = true;

                    WebBrowserDriver = new FirefoxDriver(driverService, firefoxOptions, TimeSpan.FromSeconds(60));
                    break;
                }
                catch (WebDriverException exception)
                {
                    message = message + $"Exception {attempt}:" + exception.Message;
                    if (attempt >= maxAttempts)
                    {
                        throw new WebDriverException($"Failed to start Web Browser in timely manner. - {message}");
                    }
                }
            }

            InitialiseWebDriver(url, deleteAllCookies);
        }

        private void InitialiseInternetExplorer(Uri url, bool deleteAllCookies)
        {
            var internetExplorerOptions = new InternetExplorerOptions
            {
                IntroduceInstabilityByIgnoringProtectedModeSettings = true,
                InitialBrowserUrl = "about:blank",
                EnableNativeEvents = true,
                EnsureCleanSession = true
            };

            WebBrowserDriver = new InternetExplorerDriver(internetExplorerOptions);

            InitialiseWebDriver(url, deleteAllCookies);
        }

        private void InitialiseChrome(Uri url, bool deleteAllCookies)
        {
            WebBrowserDriver = new ChromeDriver();
            InitialiseWebDriver(url, deleteAllCookies);
        }

        private void InitialiseZapChrome(Uri url, bool deleteAllCookies)
        {
            const string PROXY = "localhost:8095";
            var chromeOptions = new ChromeOptions();
            var proxy = new Proxy();
            proxy.HttpProxy = PROXY;
            proxy.SslProxy = PROXY;
            proxy.FtpProxy = PROXY;
            chromeOptions.Proxy = proxy;
            WebBrowserDriver = new ChromeDriver(chromeOptions);
            InitialiseWebDriver(url, deleteAllCookies);
        }

        private void InitialiseWebDriver(Uri url, bool deleteAllCookies)
        {
            const int maxAttempts = 3;
            for (var attempt = 1; attempt <= maxAttempts; attempt++)
            {
                var message = string.Empty;
                try
                {
                    SetTimeout.PageLoad(WebBrowserDriver, TimeSpan.FromSeconds(TimeoutInSeconds.DefaultTimeout));

                    if (deleteAllCookies)
                    {
                        WebBrowserDriver.Manage().Cookies.DeleteAllCookies();
                    }

                    WebBrowserDriver.Manage().Window.Maximize();
                    WebBrowserDriver.Navigate().GoToUrl(url);
                    break;
                }
                catch (WebDriverException exception)
                {
                    message = message + $"Exception {attempt}:" + exception.Message;
                    if (attempt >= maxAttempts)
                    {
                        throw new WebDriverException(string.Format($"Failed to start Web Browser in timely manner. - {message}"));
                    }
                }
            }
        }
    }
}
