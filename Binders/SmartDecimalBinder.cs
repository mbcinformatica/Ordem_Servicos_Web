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

            // 🔹 Remove caracteres inválidos, mas mantém vírgula e ponto
            valor = Regex.Replace(valor, @"[^0-9.,-]", "");

            // 🔹 Normaliza separador decimal:
            // Se o último separador for vírgula → decimal brasileiro
            // Se for ponto → decimal invariável
            int lastComma = valor.LastIndexOf(',');
            int lastDot = valor.LastIndexOf('.');

            if (lastComma > lastDot)
            {
                // vírgula é separador decimal → remove pontos (milhar)
                valor = valor.Replace(".", "");
            }
            else if (lastDot > lastComma)
            {
                // ponto é separador decimal → remove vírgulas (milhar)
                valor = valor.Replace(",", "");
            }

            decimal dec;

            // 🔹 Primeiro tenta com cultura brasileira
            if (decimal.TryParse(valor, NumberStyles.Number, CultureInfo.GetCultureInfo("pt-BR"), out dec))
            {
                bindingContext.Result = ModelBindingResult.Success(dec);
            }
            // 🔹 Depois tenta com cultura invariável
            else if (decimal.TryParse(valor, NumberStyles.Number, CultureInfo.InvariantCulture, out dec))
            {
                bindingContext.Result = ModelBindingResult.Success(dec);
            }
            else
            {
                // Se não conseguir converter, retorna erro de binding
                bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, $"Valor inválido: {valor}");
            }

            return Task.CompletedTask;
        }
    }
}