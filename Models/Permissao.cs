using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordem_Servicos_Web.Models
{
    [Table("DBPermissoes")]
    public class Permissao
    {
        // <summary>
        [Key]
        [Column("IDPermissao")]
        public int IdPermissao { get; set; }

        // </summary>
        [ForeignKey(nameof(Menu))]
        [Column("IDMenu")]
        public int IdMenu { get; set; }

        // </summary>
        [ForeignKey(nameof(ItensMenu))]
        [Column("IDItensMenu")]
        public int IdItensMenu { get; set; }

        // </summary>
        [ForeignKey(nameof(Usuario))]
        [Column("IDUsuario")]
        public int IdUsuario { get; set; } = 0;

        //  </summary>
        public bool Executar { get; set; } = false;

        //  </summary>
        public bool Criar { get; set; } = false;

        // </summary>
        public bool Alterar { get; set; } = false;

        // </summary>
        public bool Excluir { get; set; } = false;

        // Propriedades de navegação
        [ValidateNever]
        public virtual Menu Menu { get; set; } = null!;

        // Propriedades de navegação
        [ValidateNever]
        public virtual ItensMenu ItensMenu { get; set; } = null!;

        // Propriedades de navegação
        [ValidateNever]
        public virtual Usuario Usuario { get; set; } = null!;
    }
}