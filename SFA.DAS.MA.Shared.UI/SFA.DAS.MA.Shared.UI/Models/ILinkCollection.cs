using System;
using System.Collections.Generic;

namespace SFA.DAS.MA.Shared.UI.Models
{
    
    public interface ILinkCollection
    {
        IReadOnlyList<Link> Links { get; }
        void AddOrUpdateLink<T>(T link) where T : Link;
        void RemoveLink<T>() where T : Link;       
    }
}
