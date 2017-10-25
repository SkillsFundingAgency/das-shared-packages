using System;

namespace NuGet
{
    public abstract class Message
    {
        protected Message()
        {
            PostedDatedTime=DateTime.Now;
        }

        public string OwnerId { get; set; }

        public string HashedAccountId { get; set; }

        public DateTime PostedDatedTime { get; set; }
    }
}
