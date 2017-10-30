using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Messages
{
    public class PayeSchemeCreatedMessage : Message
    {
        public PayeSchemeCreatedMessage()
        {

        }

        public PayeSchemeCreatedMessage(string empRef, string payeScheme)
        {
            EmpRef = empRef;
            PayeScheme = payeScheme;
        }

        public string PayeScheme { get;  }

        public string EmpRef { get; }
    }
}
