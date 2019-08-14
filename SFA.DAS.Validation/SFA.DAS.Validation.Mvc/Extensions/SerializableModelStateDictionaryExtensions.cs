#if NETCOREAPP2_0
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SFA.DAS.Validation.Mvc.ModelBinding;

namespace SFA.DAS.Validation.Mvc.Extensions
{
    public static class SerializableModelStateDictionaryExtensions
    {
        public static ModelStateDictionary ToModelState(this SerializableModelStateDictionary serializableModelState)
        {
            var modelState = new ModelStateDictionary();

            foreach (var data in serializableModelState.Data)
            {
                modelState.SetModelValue(data.Key, data.RawValue, data.AttemptedValue);

                foreach (var error in data.ErrorMessages)
                {
                    modelState.AddModelError(data.Key, error);
                }
            }

            return modelState;
        }
    }
}
#elif NET462
using System.Globalization;
using System.Web.Mvc;
using SFA.DAS.Validation.Mvc.ModelBinding;

namespace SFA.DAS.Validation.Mvc.Extensions
{
    public static class SerializableModelStateDictionaryExtensions
    {
        public static ModelStateDictionary ToModelState(this SerializableModelStateDictionary serializableModelState)
        {
            var modelState = new ModelStateDictionary();

            foreach (var data in serializableModelState.Data)
            {
                ValueProviderResult value = null;
                
                if (data.RawValue != null || data.AttemptedValue != null || data.CultureName != null)
                {
                    value = new ValueProviderResult(data.RawValue, data.AttemptedValue, data.CultureName == null ? null : CultureInfo.GetCultureInfo(data.CultureName));
                }

                modelState.SetModelValue(data.Key, value);

                foreach (var error in data.ErrorMessages)
                {
                    modelState.AddModelError(data.Key, error);
                }
            }

            return modelState;
        }
    }
}
#endif