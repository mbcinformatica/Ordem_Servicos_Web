using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.RegularExpressions;

namespace Ordem_Servicos_Web.Binders
{
    public class SmartCepBinder : IModelBinder
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

            // mantém apenas dígitos
            valor = Regex.Replace(valor, @"\D", "");

            if (valor.Length != 8)
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "CEP deve conter 8 dígitos.");

            bindingContext.Result = ModelBindingResult.Success(valor);
            return Task.CompletedTask;
        }
    }
}