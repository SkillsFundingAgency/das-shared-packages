#if NET462
using System.Web.Mvc;

namespace SFA.DAS.UnitOfWork.Mvc
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