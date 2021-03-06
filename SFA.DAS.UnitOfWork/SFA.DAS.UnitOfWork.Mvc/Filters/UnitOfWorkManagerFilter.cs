﻿#if NET462
using System;
using System.Web.Mvc;
using SFA.DAS.UnitOfWork.Managers;

namespace SFA.DAS.UnitOfWork.Mvc.Filters
{
    public class UnitOfWorkManagerFilter : ActionFilterAttribute
    {
        private const string OnActionExecutedExceptionKey = "__Exception__";

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
            if (!filterContext.IsChildAction && filterContext.Exception != null)
            {
                if (filterContext.ExceptionHandled)
                {
                    filterContext.Controller.ViewData.Add(OnActionExecutedExceptionKey, filterContext.Exception);
                }
                else
                {
                    _unitOfWorkManager().EndAsync(filterContext.Exception).GetAwaiter().GetResult();
                }
            }
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (!filterContext.IsChildAction)
            {
                _unitOfWorkManager().EndAsync(filterContext.Controller.ViewData[OnActionExecutedExceptionKey] as Exception ?? filterContext.Exception).GetAwaiter().GetResult();
            }
        }
    }
}
#endif