namespace SFA.DAS.EmployerUrlHelper
{
    public interface ILinkGenerator
    {
        #region Accounts

        string Account(string accountHashedId);
        string NotificationSettings();
        string PayeSchemes(string accountHashedId);
        string RenameAccount(string accountHashedId);
        string YourAccounts();
        string YourOrganisationsAndAgreements(string accountHashedId);
        string YourTeam(string accountHashedId);

        #endregion Accounts

        #region Commitments

        string Apprentices(string accountHashedId);

        #endregion Commitments

        #region Portal

        string Help();
        string Homepage();
        string Privacy();

        #endregion Portal

        #region Recruit
        
        string Recruit(string accountHashedId);
        
        #endregion Recruit
    }
}