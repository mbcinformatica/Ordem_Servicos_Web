using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Ordem_Servicos_Web.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Ordem_Servicos_Web.Validations
{
    public class CpfCnpjAttribute : ValidationAttribute, IClientModelValidator
    {
        public string TipoPessoaProperty { get; }

        public CpfCnpjAttribute(string tipoPessoaProperty)
        {
            TipoPessoaProperty = tipoPessoaProperty;
            ErrorMessage = "Documento Inválido.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var tipoPessoaProp = validationContext.ObjectType.GetProperty(TipoPessoaProperty);
            if (tipoPessoaProp == null)
                return new ValidationResult($"Propriedade {TipoPessoaProperty} não encontrada.");

            var tipoPessoa = tipoPessoaProp.GetValue(validationContext.ObjectInstance)?.ToString();
            var documento = value?.ToString();

            if (string.IsNullOrEmpty(documento))
                return ValidationResult.Success;

            if (tipoPessoa == "FÍSICA")
            {
                if (!DocumentoHelper.ValidaCpf(documento))
                    return new ValidationResult("CPF Inválido.");
            }
            else if (tipoPessoa == "JURÍDICA")
            {
                if (!DocumentoHelper.ValidaCnpj(documento))
                    return new ValidationResult("CNPJ Inválido.");
            }

            return ValidationResult.Success;
        }

        // Client-side
        public void AddValidation(ClientModelValidationContext context)
        {
            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-cpfcnpj", ErrorMessage);
            MergeAttribute(context.Attributes, "data-val-cpfcnpj-tipopessoa", TipoPessoaProperty);
        }

        private bool MergeAttribute(IDictionary<string, string> attributes, string key, string? value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            if (attributes.ContainsKey(key)) return false;
            attributes.Add(key, value);
            return true;
        }
    }
}