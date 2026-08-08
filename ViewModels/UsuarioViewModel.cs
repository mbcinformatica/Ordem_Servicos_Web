using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Ordem_Servicos_Web.Binders;
using Ordem_Servicos_Web.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordem_Servicos_Web.ViewModels
{
    public class UsuarioViewModel

    {
        public int IdUsuario { get; set; }

        public string NomeUsuario { get; set; } = string.Empty;

        public string Login { get; set; } = string.Empty;

        public string? Senha { get; set; }

        public string? ConfirmaSenha { get; set; }

        [ModelBinder(BinderType = typeof(SmartCepBinder))]
        public string? Cep { get; set; }

        [NotMapped]
        [ValidateNever]
        public string CepFormatado => Cep != null 
            ? FormatHelper.FormatCep(Cep) 
            : string.Empty;

        public string? Endereco { get; set; }

        public string? Numero { get; set; }

        public string? Bairro { get; set; }

        public string? Municipio { get; set; }

        public string? Uf { get; set; }

        [ModelBinder(BinderType = typeof(SmartTelefoneBinder))]
        public string? FoneFixo { get; set; }

        [NotMapped]
        [ValidateNever]
        public string FoneFixoFormatado => FoneFixo != null 
            ? FormatHelper.FormatTelefone(FoneFixo) 
            : string.Empty;

        [ModelBinder(BinderType = typeof(SmartTelefoneBinder))]
        public string? FoneCelular { get; set; }

        [NotMapped]
        [ValidateNever]
        public string FoneCelularFormatado => FoneCelular != null 
            ? FormatHelper.FormatTelefone(FoneCelular) 
            : string.Empty;

        public string? Email { get; set; }

        [NotMapped]
        [ValidateNever]
        public string EmailFormatado => Email != null
            ? FormatHelper.ConverteParaMinusculo(Email)
            : string.Empty;

        public DateTime DataCadastro { get; set; }

        public byte[]? Imagem { get; set; }
    }
}