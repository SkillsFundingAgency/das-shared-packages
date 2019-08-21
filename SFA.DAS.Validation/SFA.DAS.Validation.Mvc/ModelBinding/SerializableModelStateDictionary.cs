using System;
using System.Collections.Generic;

namespace SFA.DAS.Validation.Mvc.ModelBinding
{
    [Serializable]
    public class SerializableModelStateDictionary
    {
        public ICollection<SerializableModelState> Data { get; set; } = new List<SerializableModelState>();
    }
}