using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublishReceiveSample
{
    public class SampleEvent
    {
        public DateTimeOffset Timestamp { get; set; }
        public string Id { get; set; }
    }
}
