using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Messaging
{
    public interface IMessageSubSystem
    {
        Task Enqueue(string message);
        Task<string> Dequeue();
    }
}
