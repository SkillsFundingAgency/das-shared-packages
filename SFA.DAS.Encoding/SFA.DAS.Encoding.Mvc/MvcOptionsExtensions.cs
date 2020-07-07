using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.Encoding.Mvc
{
    public static class MvcOptionsExtensions
    {
        public static void AddAutoDecoder(this MvcOptions options)
        {
            options.ModelBinderProviders.Insert(0, new AutoDecodeModelBinderProvider());
        }
    }
}
