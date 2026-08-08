using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Ordem_Servicos_Web.Binders
{
    public class SmartDecimalBinder : IModelBinder
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

            // 🔹 Remove prefixo "R$" e espaços
            valor = valor.Replace("R$", "").Trim();

            // 🔹 Remove caracteres inválidos, mas mantém dígitos, ponto e vírgula
            valor = Regex.Replace(valor, @"[^0-9.,-]", "");

            // 🔹 Substitui todas as vírgulas por pontos
            valor = valor.Replace(",", ".");

            // 🔹 Tenta converter para decimal usando cultura invariável
            if (decimal.TryParse(valor, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal dec))
            {
                // Se for campo de quantidade → força 3 casas decimais
                if (bindingContext.ModelMetadata.Name?.ToLower().Contains("quantidade") == true)
                {
                    dec = decimal.Round(dec, 3, MidpointRounding.AwayFromZero);
                }

                bindingContext.Result = ModelBindingResult.Success(dec);
            }
            else
            {
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, $"Valor inválido: {valor}");
            }

            return Task.CompletedTask;
        }
    }
}
