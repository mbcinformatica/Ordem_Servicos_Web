using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordem_Servicos_Web.Models
{
    [Table("DBModelos")]
    public class Modelo
    {
        // Chave primária
        [Key]
        [Column("IDModelo")]
        public int IdModelo { get; set; }

        // Chave estrangeira para Marca
        [ForeignKey(nameof(Marca))]
        [Column("IDMarca")]
        public int IdMarca { get; set; }

        // Propriedade de descrição do modelo
        public string Descricao { get; set; } = nameof(Descricao);

        // Propriedade de navegação
        [ValidateNever]
        public virtual Marca? Marca { get; set; }

        // Propriedade de navegação para a coleção de produtos
        public virtual ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    }
}