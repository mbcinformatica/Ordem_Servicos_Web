using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordem_Servicos_Web.Models
{
    [Table("DBMarcas")]
    public class Marca
    {
        // Chave primária
        [Key]
        [Column("IDMarca")]
        public int IdMarca { get; set; }

        // Propriedade de descrição da marca
        [Required(ErrorMessage = "Descrição da Marca é Obrigatória")]
        [StringLength(100)]
        public string Descricao { get; set; } = nameof(Descricao);

        // Propriedade de navegação
        public virtual ICollection<Modelo> Modelos { get; set; } = new List<Modelo>();
        public virtual ICollection<Produto> Produtos { get; set; } = new List<Produto>();

    }
}
