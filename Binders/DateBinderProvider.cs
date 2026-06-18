using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ordem_Servicos_Web.Binders
{

    public class DateBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(DateTime) || context.Metadata.ModelType == typeof(DateTime?))
            {
                return new DateBinder();
            }
            return null;
        }
    }
}