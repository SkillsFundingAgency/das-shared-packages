using System;
using System.ComponentModel;
using System.Globalization;
using System.Web.Mvc;

namespace SFA.DAS.Authorization.Mvc
{
    public class MessageModelBinder : DefaultModelBinder
    {
        private readonly Func<ICallerContextProvider> _callerContextProvider;

        public MessageModelBinder(Func<ICallerContextProvider> callerContextProvider)
        {
            _callerContextProvider = callerContextProvider;
        }

        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor)
        {
            if (typeof(IAccountMessage).IsAssignableFrom(bindingContext.ModelType) & propertyDescriptor.Name == nameof(IAccountMessage.AccountHashedId))
            {
                var callerContext = _callerContextProvider().GetCallerContext();
                var key = CreateSubPropertyName(bindingContext.ModelName, propertyDescriptor.Name);
                var value = callerContext.AccountHashedId;
                var valueProviderResult = new ValueProviderResult(value, value, CultureInfo.InvariantCulture);
                var model = (IAccountMessage)bindingContext.Model;

                model.AccountHashedId = value;
                bindingContext.ModelState.SetModelValue(key, valueProviderResult);

                return;
            }

            if (typeof(IAccountMessage).IsAssignableFrom(bindingContext.ModelType) & propertyDescriptor.Name == nameof(IAccountMessage.AccountId))
            {
                var callerContext = _callerContextProvider().GetCallerContext();
                var key = CreateSubPropertyName(bindingContext.ModelName, propertyDescriptor.Name);
                var value = callerContext.AccountId;
                var valueProviderResult = new ValueProviderResult(value, value?.ToString(), CultureInfo.InvariantCulture);
                var model = (IAccountMessage)bindingContext.Model;

                model.AccountId = value;
                bindingContext.ModelState.SetModelValue(key, valueProviderResult);

                return;
            }

            if (typeof(IUserMessage).IsAssignableFrom(bindingContext.ModelType) && propertyDescriptor.Name == nameof(IUserMessage.UserRef))
            {
                var callerContext = _callerContextProvider().GetCallerContext();
                var key = CreateSubPropertyName(bindingContext.ModelName, propertyDescriptor.Name);
                var value = callerContext.UserRef;
                var valueProviderResult = new ValueProviderResult(value, value?.ToString(), CultureInfo.InvariantCulture);
                var model = (IUserMessage)bindingContext.Model;

                model.UserRef = value;
                bindingContext.ModelState.SetModelValue(key, valueProviderResult);

                return;
            }

            base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
        }
    }
}