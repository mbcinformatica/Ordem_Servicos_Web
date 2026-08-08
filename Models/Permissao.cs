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

        public int? IdMenu { get; set; }

        public int? IdItensMenu { get; set; }

        public int? IdUsuario { get; set; }

        //  </summary>
        public bool Executar { get; set; } = false;

        //  </summary>
        public bool Criar { get; set; } = false;

        // </summary>
        public bool Alterar { get; set; } = false;

        // </summary>
        public bool Excluir { get; set; } = false;

        // Propriedades de navegação
        [ValidateNever] public virtual Menu? Menu { get; set; }

        // Propriedades de navegação
        [ValidateNever] public virtual ItensMenu? ItensMenu { get; set; }

        // Propriedades de navegação
        [ValidateNever] public virtual Usuario? Usuario { get; set; }
    }
}