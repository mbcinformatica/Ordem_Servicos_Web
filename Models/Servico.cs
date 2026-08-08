using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Ordem_Servicos_Web.Binders;
using Ordem_Servicos_Web.Helpers;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordem_Servicos_Web.Models
{
    [Table("DBServicos")]
    public class Servico
    {
        [Key]
        [Column("IDServico")]
        public int IdServico { get; set; }

        [Column("IDCodigoBase")]
        public string IdCodigoBase { get; set; } = string.Empty;

        [Column("IDCategoriaServico")]
        public int? IdCategoriaServico { get; set; }

        public string Descricao { get; set; } = string.Empty;

        [ModelBinder(BinderType = typeof(SmartDecimalBinder))]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = true)]
        public decimal? ValorServico { get; set; }    

        // Propriedade para exibir o valor formatado
        [NotMapped]
        [ValidateNever]
        public string ValorServicoFormatado => (ValorServico.HasValue && ValorServico.Value > 0)
            ? FormatHelper.FormatValor(ValorServico.Value)
            : string.Empty;

        // Propriedade de navegação
        [ValidateNever]
        public virtual CategoriaServico? CategoriaServico { get; set; }
    }
}
