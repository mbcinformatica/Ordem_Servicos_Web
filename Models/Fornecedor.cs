using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordem_Servicos_Web.Models
{
    [Table("DBFornecedores")]
    public class Fornecedor : PessoaFisicaJuridica
    {
        // Propriedade específica para Fornecedor
        [Key]
        [Column("IDFornecedor")]
        public int IdFornecedor { get; set; }

        // Propriedade de navegação
        public virtual ICollection<Produto>? Produtos { get; set; }

    }
}
