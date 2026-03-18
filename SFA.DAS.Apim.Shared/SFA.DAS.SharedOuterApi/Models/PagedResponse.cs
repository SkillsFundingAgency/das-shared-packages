using System.Collections.Generic;

namespace SFA.DAS.SharedOuterApi.Models
{
    public class PagedResponse<T>
    {
        public required List<T> Data { get; set; }
        public int Page { get; set; }
        public int TotalPages { get; set; }
    }
}
