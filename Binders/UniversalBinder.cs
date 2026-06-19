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

            // 🔹 Recupera também a classe CSS enviada como campo oculto
            var classInfo = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + "Class").FirstValue;
            string tipoCampo = classInfo?.ToLowerInvariant() ?? bindingContext.ModelName.ToLowerInvariant();

            // 🔹 Data
            if (tipoCampo.Contains("data"))
            {
                string[] formatos = [
                    "dd/MM/yyyy", "dd-MM-yyyy", "yyyy-MM-dd",
                    "dd/MM/yyyy HH:mm", "dd-MM-yyyy HH:mm", "yyyy-MM-dd HH:mm"
                ];

                if (DateTime.TryParseExact(valor, formatos, CultureInfo.InvariantCulture,
                                           DateTimeStyles.None, out var data))
                {
                    bindingContext.Result = ModelBindingResult.Success(data);
                }
                else
                {
                    bindingContext.ModelState.TryAddModelError(bindingContext.ModelName, "Data inválida.");
                }
            }
            else if (tipoCampo.Contains("cpf") || tipoCampo.Contains("cnpj"))
            {
                valor = Regex.Replace(valor, @"\D", "");
                bindingContext.Result = (valor.Length == 11 || valor.Length == 14)
                    ? ModelBindingResult.Success(valor)
                    : ModelBindingResult.Failed();
            }
            else if (tipoCampo.Contains("cep"))
            {
                valor = Regex.Replace(valor, @"\D", "");
                bindingContext.Result = (valor.Length == 8)
                    ? ModelBindingResult.Success(valor)
                    : ModelBindingResult.Failed();
            }
            else if (tipoCampo.Contains("telefone") || tipoCampo.Contains("fone"))
            {
                valor = Regex.Replace(valor, @"\D", "");
                bindingContext.Result = (valor.Length >= 10 && valor.Length <= 11)
                    ? ModelBindingResult.Success(valor)
                    : ModelBindingResult.Failed();
            }
            else if (tipoCampo.Contains("email"))
            {
                valor = valor.Trim().ToLowerInvariant();
                var regexEmail = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
                bindingContext.Result = regexEmail.IsMatch(valor)
                    ? ModelBindingResult.Success(valor)
                    : ModelBindingResult.Failed();
            }
            else if (tipoCampo.Contains("monetario"))
            {
                // remove tudo que não for número, vírgula ou ponto
                valor = Regex.Replace(valor, @"[^0-9.,]", "");

                // converte vírgula para ponto
                valor = valor.Replace(",", ".");

                // mantém apenas o último ponto como separador decimal
                int lastDot = valor.LastIndexOf('.');
                if (lastDot >= 0)
                {
                    var inteiro = valor.Substring(0, lastDot).Replace(".", "");
                    var decimalParte = valor.Substring(lastDot);
                    valor = inteiro + decimalParte;
                }

                if (decimal.TryParse(valor, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var dec))
                {
                    bindingContext.Result = ModelBindingResult.Success(dec);
                }
                else
                {
                    // se não conseguir converter, retorna null em vez de marcar inválido
                    bindingContext.Result = ModelBindingResult.Success(null);
                }
            }
            else if (tipoCampo.Contains("quantidade"))
            {
                valor = Regex.Replace(valor, @"\D", "");
                bindingContext.Result = decimal.TryParse(valor, NumberStyles.Any, CultureInfo.InvariantCulture, out var qtd)
                    ? ModelBindingResult.Success(qtd)
                    : ModelBindingResult.Failed();
            }
            else
            {
                // 🔹 Default: trata como string normal
                bindingContext.Result = ModelBindingResult.Success(valor.Trim());
            }

            return Task.CompletedTask;
        }
    }
}