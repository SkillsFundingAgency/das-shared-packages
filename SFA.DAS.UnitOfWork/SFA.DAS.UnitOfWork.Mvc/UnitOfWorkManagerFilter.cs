#if NET462
using System;
using System.Web.Mvc;

namespace SFA.DAS.UnitOfWork.Mvc
{
    public class UnitOfWorkManagerFilter : ActionFilterAttribute
    {
        private readonly Func<IUnitOfWorkManager> _unitOfWorkManager;

        public UnitOfWorkManagerFilter(Func<IUnitOfWorkManager> unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.IsChildAction)
            {
                _unitOfWorkManager().BeginAsync().GetAwaiter().GetResult();
            }
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!filterContext.IsChildAction)
            {
                _unitOfWorkManager().EndAsync(filterContext.Exception).GetAwaiter().GetResult();
            }
        }
    }
}
#endif