using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordem_Servicos_Web.Models
{
    [Table("DBCategoriaServicos")]
    public class CategoriaServico
    {
<<<<<<< HEAD
        // Atributos
=======
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
        [Key]
        [Column("IDCategoriaServico")]
        public int IdCategoriaServico { get; set; }

<<<<<<< HEAD
        // Propriedades
        [Required(ErrorMessage = "Descrição da Categoria de Serviços é Obrigatória")]
        [StringLength(100)]
        public string Descricao { get; set; } = string.Empty;
        // Propriedade de navegação
        public virtual ICollection<Servico> Servicos { get; set; } = new List<Servico>();
    }
}
=======
        [Required, StringLength(100)]
        public string Descricao { get; set; }
    }
}
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
