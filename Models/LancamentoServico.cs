using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordem_Servicos_Web.Models
{
    [Table("DBLancamentoServicos")]
    public class LancamentoServico
    {
<<<<<<< HEAD
        // Propriedades da classe LancamentoServico
=======
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
        [Key]
        [Column("IDOrdenServico")]
        public int IdOrdemServico { get; set; }

<<<<<<< HEAD
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
=======
        public DateTime? DataEmissao { get; set; }
        public DateTime? DataConclusao { get; set; }
        public int? IdCliente { get; set; }
        public int? IdMarca { get; set; }
        public int? IdProduto { get; set; }

        [StringLength(100)]
        public string NumeroSerie { get; set; }

        public string DescricaoDefeito { get; set; }

        [StringLength(50)]
        public string GarantiaServico { get; set; }

        [StringLength(50)]
        public string GarantiaMaterial { get; set; }

        public decimal? ValorTotalServico { get; set; }
        public decimal? ValorTotalMaterial { get; set; }

        public byte[] Imagem { get; set; }
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
    }
}
