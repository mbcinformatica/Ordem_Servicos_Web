using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ordem_Servicos_Web.Binders
{
    public class UniversalBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            // 🔹 Sempre retorna o UniversalBinder para qualquer tipo
            return new UniversalBinder();
        }
    }
}