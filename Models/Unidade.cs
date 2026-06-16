using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordem_Servicos_Web.Models
{
    [Table("DBUnidades")]
    public class Unidade
    {
        [Key]
        [Column("IDUnidade")]
        public int IdUnidade { get; set; }

<<<<<<< HEAD
        [Required(ErrorMessage = "Descrição da Unidade é Obrigatória")]
        [StringLength(100)]
        public string? Descricao { get; set; }

        // Propriedade de navegação
        public virtual ICollection<Produto> Produtos { get; set; } = new List<Produto>();

=======
        [Required, StringLength(100)]
        public string Descricao { get; set; }
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
    }
}
