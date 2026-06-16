using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordem_Servicos_Web.Models
{
    [Table("DBItensMenu")]
    public class ItensMenu
    {
        // Propriedades
        [Key]
        public int IdItensMenu { get; set; }

        // Chave estrangeira para Menu
        [ForeignKey("Menu")]
        public int? IdMenu { get; set; } = null;

        // Propriedade de descrição do item de menu
        [Required(ErrorMessage = "Descrição do Items de Menu é Obrigatório")]
        [StringLength(100)]
        public string Descricao { get; set; } = string.Empty;

        // Navegação
        [ValidateNever]
        public virtual Menu? Menu { get; set; }

        // Navegação para Permissoes (relacionamento muitos-para-muitos)
        [ValidateNever]
        public virtual ICollection<Permissao> Permissoes { get; set; } = new List<Permissao>();
    }
}