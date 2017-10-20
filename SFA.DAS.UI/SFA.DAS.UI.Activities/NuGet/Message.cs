using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet
{
    public abstract class Message
    {
        public string OwnerId { get; set; }

        public string Url { get; set; }

        public DateTime PostedDatedTime { get; set; }
    }
}
