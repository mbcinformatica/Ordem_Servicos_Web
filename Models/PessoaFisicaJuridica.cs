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
        public string TipoPessoa { get; set; } = string.Empty;

        [CpfCnpj("TipoPessoa")] // Validação customizada
        [Column("Cpf_Cnpj")]
        public string CpfCnpj { get; set; } = string.Empty;

        // Propriedade para exibir o CPF/CNPJ formatado, sem mapear para o banco de dados e sem validação.
        [NotMapped]
        public string CpfCnpjFormatado => CpfCnpj != null ? FormatHelper.FormatCpfCnpj(CpfCnpj, TipoPessoa) : string.Empty;

        // O campo "NomeRazaoSocial" é obrigatório tanto para Pessoa Física quanto para Pessoa Jurídica, então a validação é aplicada diretamente aqui.
        [Column("Nome_RazaoSocial")]
        public string NomeRazaoSocial { get; set; } = string.Empty;

        // O campo "NomeFantasia" é opcional e só é relevante para Pessoa Jurídica, então a validação é aplicada diretamente aqui.
        public string? Endereco { get; set; } = string.Empty;

        // O campo "Numero" é opcional, mas se fornecido, deve ter no máximo 10 caracteres.
        public string? Numero { get; set; }

        // O campo "Complemento" é opcional e pode ter no máximo 50 caracteres.
        public string? Bairro { get; set; } = string.Empty;

        // O campo "Municipio" é opcional e pode ter no máximo 100 caracteres.
        public string? Municipio { get; set; } = string.Empty;

        // O campo "Uf" é opcional, mas se fornecido, deve ter exatamente 2 caracteres (sigla do estado).
        [Column("UF")]
        public string? Uf { get; set; }

        public string? Cep { get; set; }

        [NotMapped]
        public string CepFormatado => Cep != null ? FormatHelper.FormatCep(Cep) : string.Empty;

        // O campo "Contato" é opcional e pode ter no máximo 50 caracteres.
        public string? Contato { get; set; }

        [Column("Fone_1")]
        public string? FoneFixo { get; set; }

        [NotMapped]
        public string FoneFixoFormatado => FoneFixo != null ? FormatHelper.FormatTelefone(FoneFixo) : string.Empty;

        [Column("Fone_2")]
        public string? FoneCelular { get; set; }

        [NotMapped]
        public string FoneCelularFormatado => FoneCelular != null ? FormatHelper.FormatTelefone(FoneCelular) : string.Empty;

        // O campo "Email" é opcional, mas se fornecido, deve ter no máximo 100 caracteres e deve ser um endereço de email válido.
        public string? Email { get; set; } = string.Empty;

        // Propriedade para exibir o email formatado, sem mapear para o banco de dados e sem validação.
        [NotMapped]
        [ValidateNever]
        public string EmailFormatado => Email != null ? FormatHelper.ConverteParaMinusculo(Email) : string.Empty;

        // O campo "DataCadastro" é obrigatório e deve ser validado para garantir que seja preenchido corretamente. A validação "Required" é aplicada para garantir que o campo seja preenchido, e a validação "DataCadastro" é aplicada para garantir que a data seja válida e não seja uma data futura.
        public DateTime DataCadastro { get; set; }
    }
}
