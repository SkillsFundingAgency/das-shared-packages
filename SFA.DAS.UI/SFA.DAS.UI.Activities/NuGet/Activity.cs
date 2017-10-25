using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet
{
    public class Activity
    {
        public string OwnerId { get; set; }

        
        public string Type { get; set; }

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
            public const string AccountAdded = "AccountAdded";
            public const string ApprenticeChangesApproved = "ApprenticeChangesApproved";
            public const string ApprenticeChangesRequested = "ApprenticeChangesRequested";
            public const string CohortApproved = "CohortApproved";

            public const string ActivityOne = "ActivityOne";
            public const string ActivityTwo = "ActivityTwo";
            public const string ActivityThree = "ActivityThree";
            public const string ActivityFour = "ActivityFour";
            public const string ActivityFive = "ActivityFive";
        }
    }
}
