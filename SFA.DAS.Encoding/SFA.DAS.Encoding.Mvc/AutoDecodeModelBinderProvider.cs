using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace SFA.DAS.Encoding.Mvc
{
    public class AutoDecodeModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var holderType = context.Metadata.ContainerType;
            if (holderType != null)
            {
                var propertyType = holderType.GetProperty(context.Metadata.PropertyName);
                var attributes = propertyType.GetCustomAttributes(true);
                if (attributes.Cast<Attribute>().Any(a => a.GetType().IsEquivalentTo(typeof(AutoDecode))))
                {
                    return new AutoDecodeModelBinder(context.Services.GetService<IEncodingService>(), context.Services.GetService<IAutoDecodeMappingProvider>());
                }
            }

            return null;
        }
    }
}
