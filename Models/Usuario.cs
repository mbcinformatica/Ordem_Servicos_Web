using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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

        [Required(ErrorMessage = "Nome do Usuário é Obrigatório")]
        [Column("Nome")]
        [StringLength(150)]
        public string NomeUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "Login é Obrigatório")]
        [Column("Login")]
        [StringLength(50)]
        public string Login { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é Obrigatória")]
        [Column("Senha")]
        [StringLength(50)]
        [DataType(DataType.Password)]
        public string Senha { get; set; } = string.Empty;

        [NotMapped]
        [Required(ErrorMessage = "Confirmação de senha é Obrigatória")]
        [DataType(DataType.Password)]
        [Compare("Senha", ErrorMessage = "As senhas não conferem")]
        public string? ConfirmaSenha { get; set; } = string.Empty;


        [StringLength(8)]
        [RegularExpression(@"^\d{8}$")]
        public string Cep { get; set; } = string.Empty;

        [NotMapped]
        public string CepFormatado => Cep != null ? FormatHelper.FormatCep(Cep) : string.Empty;

        [StringLength(100)]
        public string Endereco { get; set; } = string.Empty;

        [StringLength(10)]
        public string? Numero { get; set; }

        [StringLength(50)]
        public string Bairro { get; set; } = string.Empty;

        [StringLength(100)]
        public string Municipio { get; set; } = string.Empty;

        [Column("UF")]
        [StringLength(2)]        
        public string Uf { get; set; } = string.Empty;


        [Column("Fone_1")]
        [StringLength(11)]
        [RegularExpression(@"^\d{10,11}$")]
        public string? FoneFixo { get; set; }

        [NotMapped]
        public string FoneFixoFormatado => FoneFixo != null ? FormatHelper.FormatTelefone(FoneFixo) : string.Empty;

        [Column("Fone_2")]
        [StringLength(11)]
        [RegularExpression(@"^\d{10,11}$")]
        public string? FoneCelular { get; set; }

        [NotMapped]
        public string FoneCelularFormatado => FoneCelular != null ? FormatHelper.FormatTelefone(FoneCelular) : string.Empty;

        [StringLength(100)]
        public string? Email { get; set; }

        [NotMapped]
        public string EmailFormatado => Email != null ? FormatHelper.ConverteParaMinusculo(Email) : string.Empty;

        public DateTime DataCadastro { get; set; }

        public byte[]? Imagem { get; set; }

        // Navegação
        [ValidateNever]
        public virtual ICollection<Permissao>? Permissoes { get; set; }
        public virtual ICollection<Log>? Logs { get; set; }

    }
}
