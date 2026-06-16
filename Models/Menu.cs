using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Ordem_Servicos_Web.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordem_Servicos_Web.Models
{
    [Table("DBMenu")]
    public class Menu
    {
        // Propriedades
        [Key]
        public int IdMenu { get; set; }

        // O campo "Descricao" é obrigatório e tem um limite de 100 caracteres
        [Required(ErrorMessage = "Descrição do Menu é Obrigatória")]
        [StringLength(100)]
        public string Descricao { get; set; } = string.Empty;

        // Navegação
        [ValidateNever]
        public virtual ICollection<ItensMenu> ItensMenu { get; set; } = new List<ItensMenu>();

        // A propriedade "Permissoes" é uma coleção de objetos do tipo "Permissao" e é marcada com o atributo "ValidateNever", indicando que ela não deve ser validada durante a validação do modelo.
        [ValidateNever]
        public virtual ICollection<Permissao> Permissoes { get; set; } = new List<Permissao>();
    }
}