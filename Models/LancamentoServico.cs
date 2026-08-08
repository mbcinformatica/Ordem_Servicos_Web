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
        public DateTime? DataEmissao { get; set; }
        
        // Outras propriedades da classe
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
        public string? NumeroSerie { get; set; }

        // Outras propriedades da classe
        public string? DescricaoDefeito { get; set; }

        // Outras propriedades da classe
        public string? GarantiaServico { get; set; }

        // Outras propriedades da classe
        public string? GarantiaMaterial { get; set; }

        // Outras propriedades da classe
        public decimal? ValorTotalServico { get; set; }

        // Outras propriedades da classe
        public decimal? ValorTotalMaterial { get; set; }

        // Outras propriedades da classe
        public byte[]? Imagem { get; set; }
    }
}
