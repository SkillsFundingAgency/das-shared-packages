using System;

namespace SFA.DAS.Authorization
{
    public enum Role
    {
        None = 0,
        Owner = 1,
        Transactor = 2,
        Viewer = 3
    }

    public static class RoleStrings {
        public static string GetRoleDescription(Role role) { return GetRoleDescription(role.ToString()); }
        public static string GetRoleDescription(string role)
        {
            switch(role)
            {
                case "Owner": return "Accept agreements, view information and manage PAYE schemes, organisations, apprentices and team members";
                case "Transactor": return "Add apprentices and view information";
                case "Viewer": return "View information but can’t make changes";
                default: throw new ArgumentException("Unexpected role: " + role);
            }
        }
        
        public static string GetRoleDescriptionToLower(Role role) { return GetRoleDescriptionToLower(role.ToString()); }
        public static string GetRoleDescriptionToLower(string role)
        {
            var str = GetRoleDescription(role);
            return char.ToLower(str[0]) + str.Substring(1);
        }


    }
}