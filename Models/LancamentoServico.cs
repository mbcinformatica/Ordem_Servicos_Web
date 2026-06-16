using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordem_Servicos_Web.Models
{
    [Table("DBLancamentoServicos")]
    public class LancamentoServico
    {
        // Propriedades da classe LancamentoServico
        [Key]
        [Column("IDOrdenServico")]
        public int IdOrdemServico { get; set; }

        // Outras propriedades da classe
        [Required(ErrorMessage = "Data de Emissão é Obrigatória")]
        public DateTime? DataEmissao { get; set; }
        
        // Outras propriedades da classe
        [Required(ErrorMessage = "Data de Conclusão é Obrigatória")]
        public DateTime? DataConclusao { get; set; }

        // Outras propriedades da classe
        [Column("IDCliente")]
        public int? IdCliente { get; set; }

        // Outras propriedades da classe
        [Column("IDMarca")]
        public int? IdMarca { get; set; }

        // Outras propriedades da classe
        [Column("IDProduto")]
        public int? IdProduto { get; set; }

        // Outras propriedades da classe
        [StringLength(100)]
        public string? NumeroSerie { get; set; }

        // Outras propriedades da classe
        [Required(ErrorMessage = "Descrição do Defeito é Obrigatória")]
        public string? DescricaoDefeito { get; set; }

        // Outras propriedades da classe
        [StringLength(50)]
        public string? GarantiaServico { get; set; }

        // Outras propriedades da classe
        [StringLength(50)]
        public string? GarantiaMaterial { get; set; }

        // Outras propriedades da classe
        public decimal? ValorTotalServico { get; set; }

        // Outras propriedades da classe
        public decimal? ValorTotalMaterial { get; set; }

        // Outras propriedades da classe
        public byte[]? Imagem { get; set; }
    }
}
