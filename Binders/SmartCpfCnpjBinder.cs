using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.RegularExpressions;

namespace Ordem_Servicos_Web.Binders
{
    public class SmartCpfCnpjBinder : IModelBinder
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

            if (valor.Length == 11)
            {
                if (!ValidaCpf(valor))
                    bindingContext.ModelState.AddModelError(bindingContext.ModelName, "CPF inválido.");
            }
            else if (valor.Length == 14)
            {
                if (!ValidaCnpj(valor))
                    bindingContext.ModelState.AddModelError(bindingContext.ModelName, "CNPJ inválido.");
            }
            else
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Documento deve ser CPF (11 dígitos) ou CNPJ (14 dígitos).");
            }

            bindingContext.Result = ModelBindingResult.Success(valor);
            return Task.CompletedTask;
        }

        private bool ValidaCpf(string cpf)
        {
            // implementar validação de CPF
            return true;
        }

        private bool ValidaCnpj(string cnpj)
        {
            // implementar validação de CNPJ
            return true;
        }
    }
}