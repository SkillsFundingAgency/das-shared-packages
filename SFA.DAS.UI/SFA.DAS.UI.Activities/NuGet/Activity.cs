using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;

namespace NuGet
{
    public class Activity
    {
        public string OwnerId { get; set; }

        
        public string TypeOfActivity { get; set; }

        [Keyword(NullValue = "null")]
        public string TypeOfActivityKeyword => TypeOfActivity;

        public string DescriptionFull { get; set; }

        /// <summary>
        /// The description to be used  in for instance '1 thing happened'. The value here should be 'thing happened'
        /// </summary>
        public string DescriptionSingular{ get; set; }

        /// <summary>
        /// The description to be used  in for instance '2 things happened'. The value here should be 'things happened'
        /// </summary>
        public string DescriptionPlural { get; set; }

        public List<string> AssociatedData { get; set; } =new List<string>();

        public string Url { get; set; }

        public DateTime PostedDateTime { get;  set; }

        [Keyword(NullValue = "null")]
        public DateTime PostedDateTimeKeyword => PostedDateTime;

        public string HashedAccountId { get;  set; }

        //public enum ActivityType
        //{
        //    ActivityOne,
        //    ActivityTwo,
        //    ActivityThree,
        //    ActivityFour,
        //    ActivityFive,
        //    AccountAdded,
        //    ApprenticeChangesApproved,
        //    ApprenticeChangesRequested,
        //    CohortApproved
        //}

        public static class ActivityType
        {
            public const string AccountCreated = "AccountCreated";
            public const string AgreementCreated = "AgreementCreated";
            public const string AgreementSigned = "AgreementSigned";
            public const string ApprenticeChangesApproved = "ApprenticeChangesApproved";
            public const string ApprenticeChangesRequested = "Apprentice Changes Requested";
            public const string CohortApproved = "CohortApproved";
            public const string LegalEntityRemoved = "LegalEntityRemoved";
            public const string PayeSchemeCreatedy = "PayeSchemeCreated";
            public const string PayeSchemeDeleted = "PayeSchemeDeleted";
        }
    }
}
