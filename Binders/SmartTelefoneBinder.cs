using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Ordem_Servicos_Web.Binders
{
    public class SmartTelefoneBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var valor = bindingContext.ValueProvider.GetValue(bindingContext.ModelName).FirstValue;

            if (string.IsNullOrWhiteSpace(valor))
            {
                bindingContext.Result = ModelBindingResult.Success(null);
                return Task.CompletedTask;
            }

            valor = Regex.Replace(valor, @"\D", "");

            if (valor.Length < 10 || valor.Length > 11)
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Telefone inválido.");

            bindingContext.Result = ModelBindingResult.Success(valor);
            return Task.CompletedTask;
        }
    }
}