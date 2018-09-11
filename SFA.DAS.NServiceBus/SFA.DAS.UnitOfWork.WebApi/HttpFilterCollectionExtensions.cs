using System.Web.Http.Filters;

namespace SFA.DAS.UnitOfWork.WebApi
{
    public static class HttpFilterCollectionExtensions
    {
        public static void AddUnitOfWorkFilter(this HttpFilterCollection filters)
        {
            filters.Add(new UnitOfWorkManagerFilter());
        }
    }
}