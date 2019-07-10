using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Versioning;

namespace SFA.DAS.EmployerUrlHelper.Sample.Lib
{
    public class TestLinks
    {
        private readonly Action<string> _logger;
        private readonly Func<ConsoleKey> _input;

        public TestLinks(Action<string> output, Func<ConsoleKey> input)
        {
            _logger = output;
            _input = input;
            Config = new EmployerUrlConfiguration();
            LinkGenerator = new LinkGenerator(Config);
        }

        public EmployerUrlConfiguration Config { get; }
        public ILinkGenerator LinkGenerator { get; }

        public void ShowAllLinks(string baseUrl)
        {
            DisplayVersionNumber();

            SetBaseUrls(baseUrl);

            ShowLink(lg => lg.Account("AC#123"));
            ShowLink(lg => lg.Apprentices("AC#123"));
            ShowLink(lg => lg.CohortDetails("AC#123", "CO#123"));
            ShowLink(lg => lg.Help());
            ShowLink(lg => lg.Homepage());
            ShowLink(lg => lg.NotificationSettings());
            ShowLink(lg => lg.PayeSchemes("AC#123"));
            ShowLink(lg => lg.Privacy());
            ShowLink(lg => lg.Recruit("AC#123"));
            ShowLink(lg => lg.RenameAccount("AC#123"));
            ShowLink(lg => lg.YourAccounts());
        }

        public void ShowLink(Expression<Func<ILinkGenerator, string>> expression)
        {
            var callExpression = expression.Body as MethodCallExpression;

            if (callExpression == null)
            {
                throw new InvalidOperationException("The expression is not a method call");
            }

            var x = expression.Compile();
            var link = x.Invoke(LinkGenerator);

            Print(callExpression.Method.Name, link);
        }

        public void WaitForKey(ConsoleKey key)
        {
            _logger($"Press {key} to exit...");
            while (_input() != key)
            {
            }
        }

        private void SetBaseUrls(string baseUrl)
        {
            Config.UsersBaseUrl = $"{baseUrl}/Users";
            Config.CommitmentsV2BaseUrl = $"{baseUrl}/CommitmentsV2";
            Config.PortalBaseUrl = $"{baseUrl}/Portal";
            Config.RecruitBaseUrl = $"{baseUrl}/Recruit";
            Config.CommitmentsBaseUrl = $"{baseUrl}/Users";
            Config.AccountsBaseUrl = $"{baseUrl}/Accounts";
            Config.ProjectionsBaseUrl = $"{baseUrl}/Projections";
        }

        private void DisplayVersionNumber()
        {
            var framework = Assembly
                .GetEntryAssembly()?
                .GetCustomAttribute<TargetFrameworkAttribute>()?
                .FrameworkName;

            var stats = new
            {
                OsPlatform = System.Runtime.InteropServices.RuntimeInformation.OSDescription,
                Framework = framework
            };

            Print("OS Platform", stats.OsPlatform);
            Print(".NET Runtime", stats.Framework);
        }

        private void Print(string name, string value)
        {
            _logger($"{name,20} - {value}");
        }
    }
}
