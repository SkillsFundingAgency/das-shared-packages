#if NETCOREAPP
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.EmployerUrlHelper.Core
{
    public static class EmployerUrlHelperExtensions
    {
#region Accounts

        public static string Account(this IUrlHelper helper, string accountHashedId)
        {
            return GetLink(helper.ActionContext.HttpContext).Account(accountHashedId);
        }

        public static string NotificationSettings(this IUrlHelper helper)
        {
            return GetLink(helper.ActionContext.HttpContext).NotificationSettings();
        }

        public static string PayeSchemes(this IUrlHelper helper, string accountHashedId)
        {
            return GetLink(helper.ActionContext.HttpContext).PayeSchemes(accountHashedId);
        }

        public static string RenameAccount(this IUrlHelper helper, string accountHashedId)
        {
            return GetLink(helper.ActionContext.HttpContext).RenameAccount(accountHashedId);
        }

        public static string YourAccounts(this IUrlHelper helper)
        {
            return GetLink(helper.ActionContext.HttpContext).YourAccounts();
        }

        public static string YourOrganisationsAndAgreements(this IUrlHelper helper, string accountHashedId)
        {
            return GetLink(helper.ActionContext.HttpContext).YourOrganisationsAndAgreements(accountHashedId);
        }

        public static string YourTeam(this IUrlHelper helper, string accountHashedId)
        {
            return GetLink(helper.ActionContext.HttpContext).YourTeam(accountHashedId);
        }
#endregion Accounts

#region Commitments
        public static string Apprentices(this IUrlHelper helper, string accountHashedId)
        {
            return GetLink(helper.ActionContext.HttpContext).Apprentices(accountHashedId);
        }
        public static string CohortDetails(this IUrlHelper helper, string accountHashedId, string commitmentHashedId)
        {
            return GetLink(helper.ActionContext.HttpContext).CohortDetails(accountHashedId, commitmentHashedId);
        }
#endregion Commitments

#region Portal
        public static string Help(this IUrlHelper helper)
        {
            return GetLink(helper.ActionContext.HttpContext).Help();
        }

        public static string Homepage(this IUrlHelper helper)
        {
            return GetLink(helper.ActionContext.HttpContext).Homepage();
        }

        public static string Privacy(this IUrlHelper helper)
        {
            return GetLink(helper.ActionContext.HttpContext).Privacy();
        }
#endregion Portal

#region Recruit
        public static string Recruit(this IUrlHelper helper, string accountHashedId)
        {
            return GetLink(helper.ActionContext.HttpContext).Recruit(accountHashedId);
        }
#endregion

        private static ILinkGenerator GetLink(HttpContext httpContext)
        { 
            return ServiceLocator.Get<ILinkGenerator>(httpContext);
        }
    }
}
#endif