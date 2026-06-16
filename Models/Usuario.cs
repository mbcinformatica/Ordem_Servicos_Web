<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Ordem_Servicos_Web.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
=======
﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Ordem_Servicos_Web.Helpers;
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7

namespace Ordem_Servicos_Web.Models
{
    [Table("DBUsuarios")]
    public class Usuario
    {
        [Key]
        [Column("IDUsuario")]
        public int IdUsuario { get; set; }

<<<<<<< HEAD
        [Required(ErrorMessage = "Nome do Usuário é Obrigatório")]
=======
        [Required(ErrorMessage = "Nome do Usuário é obrigatório")]
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
        [Column("Nome")]
        [StringLength(150)]
        public string NomeUsuario { get; set; } = string.Empty;

<<<<<<< HEAD
        [Required(ErrorMessage = "Login é Obrigatório")]
=======
        [Required(ErrorMessage = "Login é obrigatório")]
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
        [Column("Login")]
        [StringLength(50)]
        public string Login { get; set; } = string.Empty;

<<<<<<< HEAD
        [Required(ErrorMessage = "Senha é Obrigatória")]
=======
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
        [Column("Senha")]
        [StringLength(50)]
        [DataType(DataType.Password)]
        public string Senha { get; set; } = string.Empty;

<<<<<<< HEAD
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
=======
        // 🔹 Apenas para validação, não mapeada no BD
        [NotMapped]
        [DataType(DataType.Password)]
        [Compare("Senha", ErrorMessage = "As senhas não conferem")]
        public string? ConfirmaSenha { get; set; }

        [StringLength(8, ErrorMessage = "CEP deve ter 8 dígitos")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "CEP inválido")]
        public string Cep { get; set; } = string.Empty;

        [NotMapped]
        public string CepFormatado => Formatador.FormatCep(Cep);
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7

        [StringLength(100)]
        public string Endereco { get; set; } = string.Empty;

        [StringLength(10)]
<<<<<<< HEAD
        public string? Numero { get; set; }
=======
        public string Numero { get; set; } = string.Empty;
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7

        [StringLength(50)]
        public string Bairro { get; set; } = string.Empty;

        [StringLength(100)]
        public string Municipio { get; set; } = string.Empty;

        [Column("UF")]
<<<<<<< HEAD
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

=======
        [StringLength(2, ErrorMessage = "UF deve ter 2 caracteres")]
        public string Uf { get; set; } = string.Empty;

        [Column("Fone_1")]
        [StringLength(11, ErrorMessage = "Telefone deve ter 10 ou 11 dígitos")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Telefone inválido")]
        public string Fone1 { get; set; } = string.Empty;

        [NotMapped]
        public string Fone1Formatado => Formatador.FormatTelefone(Fone1);

        [Column("Fone_2")]
        [StringLength(11, ErrorMessage = "Telefone deve ter 10 ou 11 dígitos")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Telefone inválido")]
        public string Fone2 { get; set; } = string.Empty;

        [NotMapped]
        public string Fone2Formatado => Formatador.FormatTelefone(Fone2);

        [StringLength(100)]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; } = string.Empty;

        [NotMapped]
        public string EmailFormatado => Formatador.ConverteParaMinusculo(Email);

        [Required(ErrorMessage = "Data de cadastro é obrigatória")]
        public DateTime DataCadastro { get; set; }

        public byte[]? Imagem { get; set; }
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
    }
}
