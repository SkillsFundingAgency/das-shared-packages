using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SFA.DAS.Encoding.Mvc
{
    public class AutoDecodeModelBinder : IModelBinder
    {
        private readonly IEncodingService _encodingService;
        private readonly IAutoDecodeMappingProvider _autoDecodeMappingProvider;

        public AutoDecodeModelBinder(IEncodingService encodingService, IAutoDecodeMappingProvider autoDecodeMappingProvider)
        {
            _encodingService = encodingService;
            _autoDecodeMappingProvider = autoDecodeMappingProvider;
        }

        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var modelName = bindingContext.ModelName;

            var mapping = _autoDecodeMappingProvider.GetAutoDecodeMapping(modelName);

            if (mapping == null)
            {
                throw new InvalidOperationException($"Unable to Auto Decode property '{modelName}' - no entry was found in AutoDecodeContextProvider with a matching key");
            }

            var encodedValueProviderResult = bindingContext.ValueProvider.GetValue(mapping.EncodedProperty);

            if (_encodingService.TryDecode(encodedValueProviderResult.FirstValue, mapping.EncodingType, out var decodedValue))
            {
                bindingContext.Result = ModelBindingResult.Success(decodedValue);
                return Task.CompletedTask;
            }

            throw new InvalidOperationException($"Unable to Auto Decode property '{modelName}' - an exception occurred while decoding the encoded value");
        }
    }
}
