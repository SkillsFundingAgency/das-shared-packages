using System;
using System.Collections.Generic;

namespace SFA.DAS.Validation.Mvc
{
    [Serializable]
    public class SerializableModelState
    {
        public string AttemptedValue { get; set; }
        public string CultureName { get; set; }
        public ICollection<string> ErrorMessages { get; set; } = new List<string>();
        public string Key { get; set; }
        public object RawValue { get; set; }
    }
}