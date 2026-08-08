using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ordem_Servicos_Web.Binders
{
    public class SmartBinderProvider : IModelBinderProvider
    {
        public IModelBinder? GetBinder(ModelBinderProviderContext context)
        {
            var propName = context.Metadata.PropertyName?.ToLower();

            // CEP
            if (context.Metadata.ModelType == typeof(string) && propName?.Contains("cep") == true)
                return new SmartCepBinder();

            // Telefone
            if (context.Metadata.ModelType == typeof(string) && propName?.Contains("telefone") == true)
                return new SmartTelefoneBinder();

            // CPF/CNPJ
            if (context.Metadata.ModelType == typeof(string) &&
                (propName?.Contains("cpf") == true || propName?.Contains("cnpj") == true || propName?.Contains("documento") == true))
                return new SmartCpfCnpjBinder();

            // Decimal
            if (context.Metadata.ModelType == typeof(decimal) || context.Metadata.ModelType == typeof(decimal?))
                return new SmartDecimalBinder();

            // DateTime
            if (context.Metadata.ModelType == typeof(DateTime) || context.Metadata.ModelType == typeof(DateTime?))
                return new SmartDateBinder();

            return null;
        }
    }
}