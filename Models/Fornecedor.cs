<<<<<<< HEAD
﻿using System.ComponentModel.DataAnnotations;
=======
﻿using System;
using System.ComponentModel.DataAnnotations;
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordem_Servicos_Web.Models
{
    [Table("DBFornecedores")]
<<<<<<< HEAD
    public class Fornecedor : PessoaFisicaJuridica
    {
        // Propriedade específica para Fornecedor
=======
    public class Fornecedor
    {
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
        [Key]
        [Column("IDFornecedor")]
        public int IdFornecedor { get; set; }

<<<<<<< HEAD
        // Propriedade de navegação
        public virtual ICollection<Produto>? Produtos { get; set; }

=======
        [Required, StringLength(10)]
        public string TipoPessoa { get; set; }

        [Required, Column("Cpf_Cnpj"), StringLength(14)]
        public string CpfCnpj { get; set; }

        [Required, Column("Nome_RazaoSocial"), StringLength(150)]
        public string NomeRazaoSocial { get; set; }

        [StringLength(100)]
        public string Endereco { get; set; }

        [StringLength(10)]
        public string Numero { get; set; }

        [StringLength(50)]
        public string Bairro { get; set; }

        [StringLength(100)]
        public string Municipio { get; set; }

        [Column("UF"), StringLength(2)]
        public string Uf { get; set; }

        [StringLength(8)]
        public string Cep { get; set; }

        [StringLength(50)]
        public string Contato { get; set; }

        [Column("Fone_1"), StringLength(11)]
        public string Fone1 { get; set; }

        [Column("Fone_2"), StringLength(10)]
        public string Fone2 { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        public DateTime DataCadastro { get; set; }
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
    }
}
