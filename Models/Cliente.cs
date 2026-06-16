using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
<<<<<<< HEAD
=======
using Ordem_Servicos_Web.Helpers;
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7

namespace Ordem_Servicos_Web.Models
{
    [Table("DBClientes")]
<<<<<<< HEAD
    public class Cliente : PessoaFisicaJuridica
    {
        // Propriedade para o ID do cliente, que é a chave primária
        [Key]
        [Column("IDCliente")]
        public int IdCliente { get; set; }
=======
    public class Cliente
    {
        [Key]
        [Column("IDCliente")]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "Tipo de Pessoa é Obrigatório")]
        [StringLength(10)]
        public string TipoPessoa { get; set; }

        [Required(ErrorMessage = "CPF/CNPJ é Obrigatório")]
        [Column("Cpf_Cnpj")]
        [StringLength(14, ErrorMessage = "CPF deve ter 11 ou CNPJ 14 Dígitos")]
        [RegularExpression(@"^\d{11}$|^\d{14}$", ErrorMessage = "CPF deve ter 11 dígitos ou CNPJ 14 dígitos")]
        public string CpfCnpj { get; set; }

        // Exibição formatada
        [NotMapped]
        public string CpfCnpjFormatado => Formatador.FormatCpfCnpj(CpfCnpj, TipoPessoa);

        [Required(ErrorMessage = "Nome/Razão Social é Obrigatório")]
        [Column("Nome_RazaoSocial")]
        [StringLength(150)]
        public string NomeRazaoSocial { get; set; }

        [StringLength(100)]
        public string Endereco { get; set; }

        [StringLength(10)]
        public string Numero { get; set; }

        [StringLength(50)]
        public string Bairro { get; set; }

        [StringLength(100)]
        public string Municipio { get; set; }

        [Column("UF")]
        [StringLength(2, ErrorMessage = "UF deve ter 2 Caracteres")]
        public string Uf { get; set; }

        [StringLength(8, ErrorMessage = "CEP deve ter 8 Dígitos")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "CEP Inválido")]
        public string Cep { get; set; }
        // Exibição formatada
        [NotMapped]
        public string CepFormatado => Formatador.FormatCep(Cep);

        [StringLength(50)]
        public string Contato { get; set; }

        [Column("Fone_1")]
        [StringLength(11, ErrorMessage = "Telefone deve ter 10 ou 11 Dígitos")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Telefone Inválido")]
        public string Fone1 { get; set; }
        // Exibição formatada
        [NotMapped]
        public string Fone1Formatado => Formatador.FormatTelefone(Fone1);

        [Column("Fone_2")]
        [StringLength(11, ErrorMessage = "Telefone deve ter 10 ou 11 Dígitos")]
        [RegularExpression(@"^\d{10,11}$", ErrorMessage = "Telefone Inválido")]
        public string Fone2 { get; set; }
        // Exibição formatada
        [NotMapped]
        public string Fone2Formatado => Formatador.FormatTelefone(Fone2);

        [StringLength(100)]
        [EmailAddress(ErrorMessage = "E-mail Inválido")]
        public string Email { get; set; }
        // Exibição formatada
        [NotMapped]
        public string EmailFormatado => Formatador.ConverteParaMinusculo(Email);

        [Required(ErrorMessage = "Data de Cadastro é Obrigatória")]
        public DateTime DataCadastro { get; set; }
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
    }
}
