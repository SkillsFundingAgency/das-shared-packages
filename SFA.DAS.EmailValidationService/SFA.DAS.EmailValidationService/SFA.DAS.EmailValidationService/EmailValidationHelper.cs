using System;
using System.Text.RegularExpressions;

namespace SFA.DAS.EmailValidationService
{
    public static class EmailValidationHelper
    {
        // take from https://www.rhyous.com/2010/06/15/csharp-email-regular-expression/
        const string EmailPattern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                                 + "@"
                                 + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))\z";
        public static bool IsAValidEmailAddress(this string emailAsString)
        {
            try
            {
                var email = new System.Net.Mail.MailAddress(emailAsString);

                // check it contains a top level domain
                var parts = email.Address.Split('@');
                if (!IsValidDomain(parts[1])) return false;

                if (!ValidByRegEx(emailAsString)) return false;

                return email.Address == emailAsString;
            }
            catch
            {
                return false;
            }
        }

        private static bool ValidByRegEx(string emailAsString)
        {
            return Regex.IsMatch(emailAsString, EmailPattern);
        }

        private static bool IsValidDomain(string domainName)
        {
            var check = Uri.CheckHostName(domainName);

            if (check == UriHostNameType.Dns)
            {
                return true;
            }

            return false;
        }
    }
}
