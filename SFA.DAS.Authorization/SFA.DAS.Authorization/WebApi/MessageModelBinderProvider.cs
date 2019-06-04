using System;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace SFA.DAS.Authorization.WebApi
{
    public class MessageModelBinderProvider : ModelBinderProvider
    {
        public override IModelBinder GetBinder(HttpConfiguration configuration, Type modelType)
        {
            return new MessageModelBinder();
        }
    }
}