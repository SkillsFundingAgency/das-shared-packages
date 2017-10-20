using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet
{
    public class Activity
    {
        public Activity()
        {
            
        }

        //public Activity(string ownerId, string activityType, string description, string url, string postedDate)
        //{
        //    OwnerId = ownerId;
        //    ActivityType = activityType;
        //    Description = description;
        //    Url = url;
        //    PostedDateTime = DateTime.Parse(postedDate, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        //}

        public string OwnerId { get; internal set; }

        public ActivityType Type { get; internal set; }

        public string DescriptionFull { get; internal set; }

        /// <summary>
        /// The description to be used  in for instance '1 thing happened'. The value here should be 'thing happened'
        /// </summary>
        public string DescriptionSingular{ get; internal set; }

        /// <summary>
        /// The description to be used  in for instance '2 things happened'. The value here should be 'things happened'
        /// </summary>
        public string DescriptionPlural { get; internal set; }

        public List<string> AssociatedData { get;  }=new List<string>();

        public string Url { get; internal set; }

        public DateTime PostedDateTime { get; internal set; }

        public enum ActivityType
        {
            ActivityOne,
            ActivityTwo,
            ActivityThree,
            ActivityFour,
            ActivityFive,
            ChangeHistory,
            CommitmentHasBeenApproved,
            ApprenticeChangesApproved
        }
    }
}
