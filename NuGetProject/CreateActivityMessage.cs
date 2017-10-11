using System;
using NuGetProject.Enums;

namespace NuGetProject
{
    public class CreateActivityMessage
    {
        public CreateActivityMessage(string accountId, ActivityType activityType, string description, string url)
        {
            AccountId = accountId;
            ActivityType = activityType;
            Description = description;
            Url = url;
            PostedDateTime = DateTime.Now.ToString("O");
        }

        public string AccountId { get;  }

        public ActivityType ActivityType { get;  }

        public string Description { get;  }

        public string Url { get;  }

        public string PostedDateTime { get;  }
    }
}
