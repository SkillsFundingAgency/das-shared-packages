using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SFA.DAS.Messaging
{
    public class Message<T>
    {
        private readonly SubSystemMessage _subSystemMessage;

        public Message(SubSystemMessage subSystemMessage)
        {
            _subSystemMessage = subSystemMessage;

            if (_subSystemMessage != null)
            {
                Content = JsonConvert.DeserializeObject<T>(_subSystemMessage.Content);
            }
        }

        public virtual T Content { get; protected set; }

        public virtual Task Complete()
        {
            return _subSystemMessage.Complete();
        }
        public virtual Task Abort()
        {
            return _subSystemMessage.Abort();
        }
    }
}
