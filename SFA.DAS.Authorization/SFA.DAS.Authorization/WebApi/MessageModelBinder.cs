using System.Globalization;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;
using WebApi.StructureMap;

namespace SFA.DAS.Authorization.WebApi
{
    public class MessageModelBinder : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            if (typeof(IAccountMessage).IsAssignableFrom(bindingContext.ModelMetadata.ContainerType) && bindingContext.ModelMetadata.PropertyName == nameof(IAccountMessage.AccountHashedId))
            {
                var callerContext = actionContext.GetService<ICallerContextProvider>().GetCallerContext();
                var key = bindingContext.ModelName;
                var value = callerContext.AccountHashedId;
                var valueProviderResult = new ValueProviderResult(value, value, CultureInfo.InvariantCulture);

                bindingContext.Model = value;
                bindingContext.ModelState.SetModelValue(key, valueProviderResult);

                return true;
            }

            if (typeof(IAccountMessage).IsAssignableFrom(bindingContext.ModelMetadata.ContainerType) && bindingContext.ModelMetadata.PropertyName == nameof(IAccountMessage.AccountId))
            {
                var callerContext = actionContext.GetService<ICallerContextProvider>().GetCallerContext();
                var key = bindingContext.ModelName;
                var value = callerContext.AccountId;
                var valueProviderResult = new ValueProviderResult(value, value?.ToString(), CultureInfo.InvariantCulture);

                bindingContext.Model = value;
                bindingContext.ModelState.SetModelValue(key, valueProviderResult);

                return true;
            }

            return false;
        }
    }
}