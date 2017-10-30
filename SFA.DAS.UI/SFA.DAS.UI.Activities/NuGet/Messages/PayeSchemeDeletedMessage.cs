using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Messages
{
    public class PayeSchemeDeletedMessage : Message
    {
        public PayeSchemeDeletedMessage()
        {

        }

        public PayeSchemeDeletedMessage(string empRef, string payeScheme)
        {
            EmpRef = empRef;
            PayeScheme = payeScheme;
        }

        public string PayeScheme { get; }

        public string EmpRef { get; }
    }
}
