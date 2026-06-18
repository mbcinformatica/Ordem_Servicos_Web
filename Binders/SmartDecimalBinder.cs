using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

            // 🔹 Se o campo for monetário (nome contém "Preco" ou "Valor")
            if (bindingContext.ModelName.Contains("Preco", StringComparison.OrdinalIgnoreCase) ||
                bindingContext.ModelName.Contains("Valor", StringComparison.OrdinalIgnoreCase))
            {
                // Remove caracteres não numéricos, exceto vírgula e ponto
                valor = Regex.Replace(valor, @"[^0-9.,]", "");
                valor = valor.Replace(",", ".");

                // Ajusta separador decimal (último ponto vira separador, os demais são removidos)
                int lastDot = valor.LastIndexOf('.');
                if (lastDot >= 0)
                {
                    var inteiro = valor[..lastDot].Replace(".", "");
                    var decimalParte = valor[lastDot..];
                    valor = inteiro + decimalParte;
                }
            }
            else
            {
                // 🔹 Para quantidade (Estoque, etc.) → apenas dígitos
                valor = Regex.Replace(valor, @"\D", "");
            }

            // 🔹 Tentativa de conversão
            if (decimal.TryParse(valor, NumberStyles.Any, CultureInfo.InvariantCulture, out var dec))
            {
                bindingContext.Result = ModelBindingResult.Success(dec);
            }
            else
            {
                // Mensagens de erro personalizadas
                if (bindingContext.ModelName.Contains("Preco", StringComparison.OrdinalIgnoreCase) ||
                    bindingContext.ModelName.Contains("Valor", StringComparison.OrdinalIgnoreCase))
                {
                    bindingContext.ModelState.TryAddModelError(bindingContext.ModelName,
                        "Preço inválido. Use apenas números, vírgula ou ponto.");
                }
                else
                {
                    bindingContext.ModelState.TryAddModelError(bindingContext.ModelName,
                        "Quantidade inválida. Digite apenas números.");
                }
            }

            return Task.CompletedTask;
        }
    }
}