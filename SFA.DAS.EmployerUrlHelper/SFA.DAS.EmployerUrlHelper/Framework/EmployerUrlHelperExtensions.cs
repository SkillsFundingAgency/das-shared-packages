using System;
#if NETFRAMEWORK
using UrlHelper =System.Web.Mvc.UrlHelper;

namespace SFA.DAS.EmployerUrlHelper.Framework
{
    public static class EmployerUrlHelperExtensions
    {
#region Accounts

        public static string Account(this UrlHelper helper, string accountHashedId)
        {
            return Link.Account(accountHashedId);
        }

        public static string NotificationSettings(this UrlHelper helper)
        {
            return Link.NotificationSettings();
        }

        public static string PayeSchemes(this UrlHelper helper, string accountHashedId)
        {
            return Link.PayeSchemes(accountHashedId);
        }

        public static string RenameAccount(this UrlHelper helper, string accountHashedId)
        {
            return Link.RenameAccount(accountHashedId);
        }

        public static string YourAccounts(this UrlHelper helper)
        {
            return Link.YourAccounts();
        }

        public static string YourOrganisationsAndAgreements(this UrlHelper helper, string accountHashedId)
        {
            return Link.YourOrganisationsAndAgreements(accountHashedId);
        }

        public static string YourTeam(this UrlHelper helper, string accountHashedId)
        {
            return Link.YourTeam(accountHashedId);
        }
#endregion Accounts

#region Commitments
        public static string Apprentices(this UrlHelper helper, string accountHashedId)
        {
            return Link.Apprentices(accountHashedId);
        }
        public static string CohortDetails(this UrlHelper helper, string accountHashedId, string commitmentHashedId)
        {
            return Link.CohortDetails(accountHashedId, commitmentHashedId);
        }
#endregion Commitments

#region Portal
        public static string Help(this UrlHelper helper)
        {
            return Link.Help();
        }

        public static string Homepage(this UrlHelper helper)
        {
            return Link.Homepage();
        }

        public static string Privacy(this UrlHelper helper)
        {
            return Link.Privacy();
        }
#endregion Portal

#region Recruit
        public static string Recruit(this UrlHelper helper, string accountHashedId)
        {
            return Link.Recruit(accountHashedId);
        }
#endregion

        private static ILinkGenerator Link => ServiceLocator.Get<ILinkGenerator>();
    }
}
#endif
