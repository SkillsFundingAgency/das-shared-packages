using System;
using System.Linq.Expressions;
using NUnit.Framework;
using SFA.DAS.EmployerUrlHelper;

namespace SFA.DAS.EmployerUrlHelper.UnitTests  
{
    public class LinkGeneratorTests
    {
        [Test]
        public void Account_ShouldGenerateExpectedUrl()
        {
            VerifyLink(lg => lg.Account("AC0123"), "/Accounts/accounts/AC0123/teams", HostSiteType.Accounts);
        }

        [Test]
        public void Apprentices_ShouldGenerateExpectedUrl()
        {
            VerifyLink(lg => lg.Apprentices("AC0123"), "/Commitments/accounts/AC0123/apprentices/home", HostSiteType.Commitments);
        }

        [Test]
        public void CohortDetails_ShouldGenerateExpectedUrl()
        {
            VerifyLink(lg => lg.CohortDetails("AC0123", "CO0123"), "/Commitments/accounts/AC0123/apprentices/CO0123/details", HostSiteType.Commitments);
        }

        [Test]
        public void Help_ShouldGenerateExpectedUrl()
        {
            VerifyLink(lg => lg.Help(), "/Portal/service/help", HostSiteType.Portal);
        }

        [Test]
        public void Homepage_ShouldGenerateExpectedUrl()
        {
            VerifyLink(lg => lg.Homepage(), "/Portal", HostSiteType.Portal);
        }

        [Test]
        public void NotificationSettings_ShouldGenerateExpectedUrl()
        {
            VerifyLink(lg => lg.NotificationSettings(), $"/{HostNames.Accounts}/settings/notifications", HostSiteType.Accounts);
        }

        [Test]
        public void PayeSchemes_ShouldGenerateExpectedUrl()
        {
            VerifyLink(lg => lg.PayeSchemes("AC0123"), $"/{HostNames.Accounts}/accounts/AC0123/schemes", HostSiteType.Accounts);
        }

        [Test]
        public void Privacy_ShouldGenerateExpectedUrl()
        {
            VerifyLink(lg => lg.Privacy(), $"/{HostNames.Portal}/service/privacy", HostSiteType.Portal);
        }

        [Test]
        public void Recruit_ShouldGenerateExpectedUrl()
        {
            VerifyLink(lg => lg.Recruit("AC0123"), "/Recruit/accounts/AC0123", HostSiteType.Recruit);
        }

        [Test]
        public void RenameAccount_ShouldGenerateExpectedUrl()
        {
            VerifyLink(lg => lg.RenameAccount("AC0123"), $"/{HostNames.Accounts}/accounts/AC0123/rename", HostSiteType.Accounts);
        }

        [Test]
        public void YourAccounts_ShouldGenerateExpectedUrl()
        {
            VerifyLink(lg => lg.YourAccounts(), $"/{HostNames.Accounts}/service/accounts", HostSiteType.Accounts);
        }

        private void VerifyLink(Func<ILinkGenerator, string> link, string expectedUrl, HostSiteType expectedHost)
        {
            var fixtures = new LinkGeneratorTestFixtures().WithBaseUrl("http://localhost:12345");
            fixtures.VerifyGeneratedLink(link, expectedUrl, expectedHost);
        }
    }

    public enum HostSiteType
    {
        Unknown,
        Users,
        CommitmentsV2,
        Portal,
        Recruit,
        Commitments,
        Accounts,
        Projections
    }


    public static class HostNames
    {
        public const string Users = "Users";
        public const string CommitmentsV2 = "CommitmentsV2";
        public const string Portal = "Portal";
        public const string Recruit = "Recruit";
        public const string Commitments = "Commitments";
        public const string Accounts = "Accounts";
        public const string Projections = "Projections";
    }

    public class LinkGeneratorTestFixtures
    {
        public LinkGeneratorTestFixtures()
        {
            Config = new EmployerUrlConfiguration();
            LinkGenerator = new LinkGenerator(Config);
        }

        public EmployerUrlConfiguration Config { get; }
        public LinkGenerator LinkGenerator { get; }

        private string _baseUrl;

        public LinkGeneratorTestFixtures WithBaseUrl(string baseUrl)
        {
            Config.UsersBaseUrl = $"{baseUrl}/{HostNames.Users}";
            Config.CommitmentsV2BaseUrl = $"{baseUrl}/{HostNames.CommitmentsV2}";
            Config.PortalBaseUrl = $"{baseUrl}/{HostNames.Portal}";
            Config.RecruitBaseUrl = $"{baseUrl}/{HostNames.Recruit}";
            Config.CommitmentsBaseUrl = $"{baseUrl}/{HostNames.Commitments}";
            Config.AccountsBaseUrl = $"{baseUrl}/{HostNames.Accounts}";
            Config.ProjectionsBaseUrl = $"{baseUrl}/{HostNames.Projections}";

            _baseUrl = baseUrl;

            return this;
        }

        public void VerifyGeneratedLink(Func<ILinkGenerator, string> link, string expectedLink, HostSiteType expectedHost)
        {
            // strip off host from the actual link
            var actualLink = link(LinkGenerator);
            var actualUrl = new Uri(actualLink, UriKind.Absolute);

            Assert.AreEqual(expectedLink, actualUrl.AbsolutePath, "The path is not the expected value");

            var actualHost = DetermineHostFromUrl(actualUrl);

            Assert.AreEqual(expectedHost, actualHost, "The host type is not the expected value");
        }

        private HostSiteType DetermineHostFromUrl(Uri link)
        {
            var pathAfterBase = link.AbsoluteUri.Substring(_baseUrl.Length+1);
            var firstPart = pathAfterBase.IndexOf('/', 0);
            var hostPart = firstPart > 0 ? pathAfterBase.Substring(0, firstPart) : pathAfterBase;

#if NETCOREAPP
            if (Enum.TryParse(typeof(HostSiteType), hostPart, true, out var hosttype))
#elif NETFRAMEWORK
            if (Enum.TryParse<HostSiteType>(hostPart, true, out var hosttype))
#endif
            {
                return (HostSiteType) hosttype;
            }

            return HostSiteType.Unknown;
        }
    }
}