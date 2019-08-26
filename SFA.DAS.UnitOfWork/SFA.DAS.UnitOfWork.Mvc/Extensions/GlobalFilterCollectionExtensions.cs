#if NET462
using System.Web.Mvc;
using SFA.DAS.UnitOfWork.Managers;
using SFA.DAS.UnitOfWork.Mvc.Filters;

namespace SFA.DAS.UnitOfWork.Mvc.Extensions
{
    public static class GlobalFilterCollectionExtensions
    {
        public static void AddUnitOfWorkFilter(this GlobalFilterCollection filters)
        {
            filters.Add(new UnitOfWorkManagerFilter(() => DependencyResolver.Current.GetService<IUnitOfWorkManager>()), -999);
        }
    }
}
#endif