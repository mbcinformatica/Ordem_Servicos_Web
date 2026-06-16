using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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

        [Required(ErrorMessage = "ID do Código Base é Obrigatório")]
        [StringLength(20)]
        [Column("IDCodigoBase")]
        public string IdCodigoBase { get; set; } = string.Empty;

        [Required(ErrorMessage = "ID da Categoria de Serviço é Obrigatório")]
        [Column("IDCategoriaServico")]
        public int? IdCategoriaServico { get; set; }

        [Required(ErrorMessage = "Descrição do Serviço é Obrigatória")]
        [StringLength(100)]
        public string Descricao { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "O valor do serviço deve ser maior que zero.")]
        public decimal? ValorServico { get; set; }    

        // Propriedade para exibir o valor formatado
        [NotMapped]
        [ValidateNever]
        public string ValorServicoFormatado => ValorServico.HasValue
            ? FormatHelper.FormatValor(ValorServico.Value)
            : string.Empty;

        // Propriedade de navegação
        [ValidateNever]
        public virtual CategoriaServico? CategoriaServico { get; set; }
    }
}
