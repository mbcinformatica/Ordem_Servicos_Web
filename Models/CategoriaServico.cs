using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordem_Servicos_Web.Models
{
    [Table("DBCategoriaServicos")]
    public class CategoriaServico
    {
        // Atributos
        [Key]
        [Column("IDCategoriaServico")]
        public int IdCategoriaServico { get; set; }

        // Propriedades
        [Required(ErrorMessage = "Descrição da Categoria de Serviços é Obrigatória")]
        [StringLength(100)]
        public string Descricao { get; set; } = string.Empty;
        // Propriedade de navegação
        public virtual ICollection<Servico> Servicos { get; set; } = new List<Servico>();
    }
}