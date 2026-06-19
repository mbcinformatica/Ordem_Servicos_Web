using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Ordem_Servicos_Web.Binders;
using Ordem_Servicos_Web.Helpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordem_Servicos_Web.Models
{
    [Table("DBProdutos")]
    public class Produto
    {
        [Key]
        [Column("IDProduto")]
        public int IdProduto { get; set; }

        public string IdProdutoInterno { get; set; } = string.Empty;
        public string IdProdutoFabricante { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;

        public int? IdFornecedor { get; set; }
        public int? IdMarca { get; set; }
        public int? IdModelo { get; set; }
        public int? IdUnidade { get; set; }

        // 🔹 Preço Compra com binder
        [ModelBinder(BinderType = typeof(SmartDecimalBinder))]
        public decimal? PrecoCompra { get; set; }

        [NotMapped]
        [ValidateNever]
        public string PrecoCompraFormatado => (PrecoCompra.HasValue && PrecoCompra.Value > 0)
            ? FormatHelper.FormatValor(PrecoCompra.Value) // R$ com 2 casas
            : string.Empty;

        // 🔹 Preço Venda com binder
        [ModelBinder(BinderType = typeof(SmartDecimalBinder))]
        public decimal? PrecoVenda { get; set; }

        [NotMapped]
        [ValidateNever]
        public string PrecoVendaFormatado => (PrecoVenda.HasValue && PrecoVenda.Value > 0)
            ? FormatHelper.FormatValor(PrecoVenda.Value)
            : string.Empty;

        // 🔹 Estoque Atual com binder
        [ModelBinder(BinderType = typeof(SmartDecimalBinder))]
        public decimal? EstoqueAtual { get; set; }

        [NotMapped]
        [ValidateNever]
        public string EstoqueAtualFormatado => (EstoqueAtual.HasValue && EstoqueAtual.Value > 0)
            ? FormatHelper.FormatQuantidade(EstoqueAtual.Value) // 3 casas decimais
            : string.Empty;

        // 🔹 Estoque Mínimo com binder
        [ModelBinder(BinderType = typeof(SmartDecimalBinder))]
        public decimal? EstoqueMinimo { get; set; }

        [NotMapped]
        [ValidateNever]
        public string EstoqueMinimoFormatado => (EstoqueMinimo.HasValue && EstoqueMinimo.Value > 0)
            ? FormatHelper.FormatQuantidade(EstoqueMinimo.Value)
            : string.Empty;

        public DateTime DataUltimaCompra { get; set; }

        [StringLength(50)]
        public string? Garantia { get; set; }

        public byte[]? Imagem { get; set; }

        // 🔹 Propriedades de navegação
        [ValidateNever] public virtual Fornecedor? Fornecedor { get; set; }
        [ValidateNever] public virtual Marca? Marca { get; set; }
        [ValidateNever] public virtual Modelo? Modelo { get; set; }
        [ValidateNever] public virtual Unidade? Unidade { get; set; }
    }
}
