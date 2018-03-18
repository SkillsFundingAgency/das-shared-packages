using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Sfa.Automation.Framework.Selenium
{
    public static class Zap
    {
        public const string Address = "http://localhost";

        private const string zapFileName = @"C:\Program Files\OWASP\Zed Attack Proxy\ZAP.exe";
        private const string zapWorkingDirextory = @"C:\Program Files\OWASP\Zed Attack Proxy";
        private static HttpClient _httpClient;
        public const int Port = 8080;
        public static string ApiKey { get; set; }

        private static Process zap;

        static Zap()
        {

        }

        public static void StartZapUI()
        {
            System.Console.WriteLine("Trying to StartZapUI");
            ProcessStartInfo zapProcessStartInfo = new ProcessStartInfo();
            zapProcessStartInfo.FileName = zapFileName;
            zapProcessStartInfo.WorkingDirectory = zapWorkingDirextory;

            System.Console.WriteLine("Issuing command to StartZapUI");
            System.Console.WriteLine(zapProcessStartInfo.ToString());
            zap = Process.Start(zapProcessStartInfo);

            CheckIfZAPHasStartedByPollingTheAPI(1);
        }

        public static void StartZAPDaemon()
        {
            System.Console.WriteLine("Trying to StartZAPDaemon");
            ProcessStartInfo zapProcessStartInfo = new ProcessStartInfo();
            zapProcessStartInfo.FileName = zapFileName;
            zapProcessStartInfo.WorkingDirectory = zapWorkingDirextory;
            zapProcessStartInfo.Arguments = $"-daemon -host 127.0.0.1 -port {Port}";

            System.Console.WriteLine("Issuing command to StartZAPDaemon");
            System.Console.WriteLine(zapProcessStartInfo.ToString());
            zap = Process.Start(zapProcessStartInfo);

            CheckIfZAPHasStartedByPollingTheAPI(1);
        }

        public static void StopZapUI()
        {
            // TODO : stop zap 
            // check if zap has stopped
        }

        private static void Sleep(int sleepTimeinSeconds)
        {
            System.Console.WriteLine($"Sleeping for {sleepTimeinSeconds} seconds");
            Thread.Sleep(sleepTimeinSeconds * 1000);
        }

        public static async Task<List<string>> GetJsonReport()
        {
            List<string> messages = new List<string>();

            _httpClient = new HttpClient { BaseAddress = new Uri($"{Address}:{Port}") };

            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/JSON/core/view/alerts/?zapapiformat=JSON&apikey={ApiKey}&formMethod=GET&baseurl=&start=&count="))
            {
                request.Headers.Add("Accept", "application/json");
                using (var response = _httpClient.SendAsync(request))
                {
                    var result = await response;
                    var content = await result.Content.ReadAsStringAsync();
                    dynamic alertsinresponse = JsonConvert.DeserializeObject(content, _jsonSettings);
                    var events = alertsinresponse.alerts;
                    foreach (var alert in events)
                    {
                        messages.Add($"{alert.alert} risk level: {alert.risk} ");
                    }
                }
            }
            return messages;
        }

        public static async Task<string> GetHtmlReport()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri($"{Address}:{Port}") };
            string content;
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/HTML/core/view/alerts/?zapapiformat=HTML&apikey={ApiKey}&formMethod=GET&baseurl=&start=&count="))
            {
                request.Headers.Add("Accept", "text/html");
                using (var response = _httpClient.SendAsync(request))
                {
                    var result = await response;
                    content = await result.Content.ReadAsStringAsync();
                }
            }
            return content;
        }

        private static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore
        };

        private static void CheckIfZAPHasStartedByPollingTheAPI(int minutesToWait)
        {
            WebClient webClient = new WebClient();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            int millisecondsToWait = minutesToWait * 60 * 1000;
            string zapUrlToDownload = $"{Address}:{Port}";

            while (millisecondsToWait > watch.ElapsedMilliseconds)
            {
                try
                {
                    System.Console.WriteLine("Trying to check if ZAP has started by accessing the ZAP API at {0}", zapUrlToDownload);
                    string responseString = webClient.DownloadString(zapUrlToDownload);
                    System.Console.WriteLine(Environment.NewLine + responseString + Environment.NewLine);
                    System.Console.WriteLine("Obtained a response from the ZAP API at {0} {1}Hence assuming that ZAP started successfully", zapUrlToDownload, Environment.NewLine);
                    return;
                }
                catch (WebException webException)
                {
                    System.Console.WriteLine("Seems like ZAP did not start yet");
                    System.Console.WriteLine(webException.Message + Environment.NewLine);
                    Sleep(2);
                }
            }

            throw new Exception(string.Format("Waited for {0} minute(s), however could not access the API successfully, hence could not verify if ZAP started successfully or not", minutesToWait));
        }

        private static void CheckIfZAPHasStopedByPollingTheAPI(int minutesToWait)
        {
            WebClient webClient = new WebClient();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            int millisecondsToWait = minutesToWait * 60 * 1000;
            string zapUrlToDownload = $"{Address}:{Port}";

            while (millisecondsToWait > watch.ElapsedMilliseconds)
            {
                try
                {
                    System.Console.WriteLine("Trying to check if ZAP has stoped by accessing the ZAP API at {0}", zapUrlToDownload);
                    string responseString = webClient.DownloadString(zapUrlToDownload);
                    System.Console.WriteLine(Environment.NewLine + responseString + Environment.NewLine);
                    System.Console.WriteLine("Obtained a response from the ZAP API at {0} {1}Hence assuming that ZAP is still running", zapUrlToDownload, Environment.NewLine);
                    Sleep(2);
                }
                catch (WebException webException)
                {
                    System.Console.WriteLine("Have not Obtained a response from the ZAP API, Seems like ZAP stopped");
                    System.Console.WriteLine(webException.Message + Environment.NewLine);
                    return;
                }
            }

            throw new Exception(string.Format("Waited for {0} minute(s), however could access the API successfully, hence could not verify if ZAP stopped successfully or not", minutesToWait));
        }
    }
}
