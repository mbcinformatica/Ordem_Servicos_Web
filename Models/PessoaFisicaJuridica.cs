using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Ordem_Servicos_Web.Helpers;
using Ordem_Servicos_Web.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordem_Servicos_Web.Models
{
    [NotMapped]
    public abstract class PessoaFisicaJuridica
    {
        // O campo "TipoPessoa" é obrigatório e deve ser validado para garantir que seja preenchido corretamente. A validação "Required" é aplicada para garantir que o campo seja preenchido, e a validação "StringLength" é aplicada para limitar o tamanho do valor a 10 caracteres, o que é suficiente para armazenar os valores "Física" ou "Jurídica".
//        [Required(ErrorMessage = "Tipo de Pessoa é Obrigatório")]
//        [StringLength(10)]
        public string TipoPessoa { get; set; } = string.Empty;

        // O campo "CpfCnpj" é obrigatório e deve ser validado de acordo com o tipo de pessoa (CPF para Pessoa Física e CNPJ para Pessoa Jurídica). A validação customizada "CpfCnpj" é aplicada para garantir que o valor seja válido conforme o tipo de pessoa.
//      [Required(ErrorMessage = "CPF/CNPJ é Obrigatório")]
        [CpfCnpj("TipoPessoa")] // Validação customizada
        [Column("Cpf_Cnpj")]
//      [StringLength(14)]
//      [RegularExpression(@"^\d{11}$|^\d{14}$")]

        public string CpfCnpj { get; set; } = string.Empty;

        // Propriedade para exibir o CPF/CNPJ formatado, sem mapear para o banco de dados e sem validação.
        [NotMapped]
        public string CpfCnpjFormatado => CpfCnpj != null ? FormatHelper.FormatCpfCnpj(CpfCnpj, TipoPessoa) : string.Empty;

        // O campo "NomeRazaoSocial" é obrigatório tanto para Pessoa Física quanto para Pessoa Jurídica, então a validação é aplicada diretamente aqui.
//        [Required(ErrorMessage = "Nome/Razão Social é Obrigatório")]
        [Column("Nome_RazaoSocial")]
//        [StringLength(150)]
        public string NomeRazaoSocial { get; set; } = string.Empty;

        // O campo "NomeFantasia" é opcional e só é relevante para Pessoa Jurídica, então a validação é aplicada diretamente aqui.
//        [StringLength(100)]
        public string? Endereco { get; set; } = string.Empty;

        // O campo "Numero" é opcional, mas se fornecido, deve ter no máximo 10 caracteres.
//        [StringLength(10)]
        public string? Numero { get; set; }

        // O campo "Complemento" é opcional e pode ter no máximo 50 caracteres.
//        [StringLength(50)]
        public string? Bairro { get; set; } = string.Empty;

        // O campo "Municipio" é opcional e pode ter no máximo 100 caracteres.
//        [StringLength(100)]
        public string? Municipio { get; set; } = string.Empty;

        // O campo "Uf" é opcional, mas se fornecido, deve ter exatamente 2 caracteres (sigla do estado).
        [Column("UF")]
//        [StringLength(2)]
        public string? Uf { get; set; } = string.Empty;

//        [StringLength(8)]
//        [RegularExpression(@"^\d{8}$")]
        public string Cep { get; set; } = string.Empty;

        [NotMapped]
        public string CepFormatado => Cep != null ? FormatHelper.FormatCep(Cep) : string.Empty;

        // O campo "Contato" é opcional e pode ter no máximo 50 caracteres.
//        [StringLength(50)]
        public string? Contato { get; set; } = string.Empty;

        [Column("Fone_1")]
//        [StringLength(11)]
//        [RegularExpression(@"^\d{10,11}$")]
        public string? FoneFixo { get; set; }

        [NotMapped]
        public string FoneFixoFormatado => FoneFixo != null ? FormatHelper.FormatTelefone(FoneFixo) : string.Empty;

        [Column("Fone_2")]
//        [StringLength(11)]
//        [RegularExpression(@"^\d{10,11}$")]
        public string? FoneCelular { get; set; }

        [NotMapped]
        public string FoneCelularFormatado => FoneCelular != null ? FormatHelper.FormatTelefone(FoneCelular) : string.Empty;

        // O campo "Email" é opcional, mas se fornecido, deve ter no máximo 100 caracteres e deve ser um endereço de email válido.
//        [StringLength(100)]
//        [EmailAddress(ErrorMessage = "Email inválido")]
        public string? Email { get; set; } = string.Empty;

        // Propriedade para exibir o email formatado, sem mapear para o banco de dados e sem validação.
        [NotMapped]
        [ValidateNever]
        public string EmailFormatado => Email != null ? FormatHelper.ConverteParaMinusculo(Email) : string.Empty;

        // O campo "DataCadastro" é obrigatório e deve ser validado para garantir que seja preenchido corretamente. A validação "Required" é aplicada para garantir que o campo seja preenchido, e a validação "DataCadastro" é aplicada para garantir que a data seja válida e não seja uma data futura.
        public DateTime DataCadastro { get; set; }
    }
}
