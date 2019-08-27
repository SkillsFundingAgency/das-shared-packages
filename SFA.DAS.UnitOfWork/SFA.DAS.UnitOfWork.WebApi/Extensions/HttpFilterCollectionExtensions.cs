using System.Web.Http.Filters;
using SFA.DAS.UnitOfWork.WebApi.Filters;

namespace SFA.DAS.UnitOfWork.WebApi.Extensions
{
    public static class HttpFilterCollectionExtensions
    {
        public static void AddUnitOfWorkFilter(this HttpFilterCollection filters)
        {
            filters.Add(new UnitOfWorkManagerFilter());
        }
    }
}