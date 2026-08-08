using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Ordem_Servicos_Web.Binders;
using Ordem_Servicos_Web.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordem_Servicos_Web.Models
{
    [Table("DBUsuarios")]
    public class Usuario
    {
        [Key]
        [Column("IDUsuario")]
        public int IdUsuario { get; set; }

        [Column("Nome")]
        public string NomeUsuario { get; set; } = string.Empty;

        [Column("Login")]
        public string Login { get; set; } = string.Empty;

        [Column("Senha")]
        [DataType(DataType.Password)]
        public string? Senha { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        [Compare("Senha", ErrorMessage = "As senhas não conferem")]
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

        [Column("UF")]
        public string? Uf { get; set; }

        [ModelBinder(BinderType = typeof(SmartTelefoneBinder))]
        [Column("Fone_1")]
        public string? FoneFixo { get; set; }

        [NotMapped]
        [ValidateNever]
        public string FoneFixoFormatado => FoneFixo != null 
            ? FormatHelper.FormatTelefone(FoneFixo) 
            : string.Empty;

        [ModelBinder(BinderType = typeof(SmartTelefoneBinder))]
        [Column("Fone_2")]
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

        // Navegação
        [ValidateNever]
        public virtual ICollection<Permissao>? Permissoes { get; set; }

        public virtual ICollection<Log>? Logs { get; set; }

    }
}