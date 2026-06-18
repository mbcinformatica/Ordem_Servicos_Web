using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;
using System.Threading.Tasks;

namespace Ordem_Servicos_Web.Binders
{

    public class DateBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);

            if (valueProviderResult == ValueProviderResult.None)
                return Task.CompletedTask;

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueProviderResult);

            var valor = valueProviderResult.FirstValue;

            if (string.IsNullOrWhiteSpace(valor))
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            // 🔹 Lista de formatos aceitos
            string[] formatos = {
                    "dd/MM/yyyy",
                    "dd-MM-yyyy",
                    "yyyy-MM-dd",
                    "dd/MM/yyyy HH:mm",
                    "dd-MM-yyyy HH:mm",
                    "yyyy-MM-dd HH:mm"
                };

            if (DateTime.TryParseExact(valor, formatos, CultureInfo.InvariantCulture,
                                       DateTimeStyles.None, out var data))
            {
                bindingContext.Result = ModelBindingResult.Success(data);
            }
            else
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Data inválida");
            }

            return Task.CompletedTask;
        }
    }
}