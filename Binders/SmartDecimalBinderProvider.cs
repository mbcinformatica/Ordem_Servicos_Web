using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ordem_Servicos_Web.Binders
{

    public class SmartDecimalBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(decimal) || context.Metadata.ModelType == typeof(decimal?))
            {
                return new SmartDecimalBinder();
            }
            return null;
        }
    }
}