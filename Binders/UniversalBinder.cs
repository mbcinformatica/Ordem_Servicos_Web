using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Ordem_Servicos_Web.Binders
{
    public class UniversalBinder : IModelBinder
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

            var targetType = bindingContext.ModelMetadata.ModelType;

            // 🔹 Inteiro
            if (targetType == typeof(int) || targetType == typeof(int?))
            {
                valor = Regex.Replace(valor, @"\D", "");
                bindingContext.Result = int.TryParse(valor, out var numero)
                    ? ModelBindingResult.Success(numero)
                    : ModelBindingResult.Success(null);
            }
            // 🔹 Decimal
            else if (targetType == typeof(decimal) || targetType == typeof(decimal?))
            {
                valor = Regex.Replace(valor, @"[^0-9.,]", "");
                valor = valor.Replace(",", ".");

                int lastDot = valor.LastIndexOf('.');
                if (lastDot >= 0)
                {
                    var inteiro = valor.Substring(0, lastDot).Replace(".", "");
                    var decimalParte = valor.Substring(lastDot);
                    valor = inteiro + decimalParte;
                }

                bindingContext.Result = decimal.TryParse(valor, NumberStyles.Any, CultureInfo.InvariantCulture, out var dec)
                    ? ModelBindingResult.Success(dec)
                    : ModelBindingResult.Success(null);
            }
            // 🔹 DateTime
            else if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
            {
                string[] formatos = {
                    "dd/MM/yyyy", "dd-MM-yyyy", "yyyy-MM-dd",
                    "dd/MM/yyyy HH:mm", "dd-MM-yyyy HH:mm", "yyyy-MM-dd HH:mm"
                };

                if (DateTime.TryParseExact(valor, formatos, CultureInfo.GetCultureInfo("pt-BR"),
                                           DateTimeStyles.None, out var data))
                {
                    bindingContext.Result = ModelBindingResult.Success(data);
                }
                else
                {
                    bindingContext.Result = ModelBindingResult.Success(null);
                }
            }
            // 🔹 String (default)
            else
            {
                bindingContext.Result = ModelBindingResult.Success(valor.Trim());
            }

            return Task.CompletedTask;
        }
    }
}