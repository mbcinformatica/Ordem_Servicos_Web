using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Ordem_Servicos_Web.Binders;
using Ordem_Servicos_Web.Helpers;
using Ordem_Servicos_Web.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordem_Servicos_Web.ViewModels
{
    public class ProdutoViewModel
    {
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
            ? FormatHelper.FormatValor(PrecoCompra.Value) // sempre R$ com 2 casas
            : string.Empty;

        // 🔹 Preço Venda com binder
        [ModelBinder(BinderType = typeof(SmartDecimalBinder))]
        public decimal? PrecoVenda { get; set; }

        [NotMapped]
        [ValidateNever]
        public string PrecoVendaFormatado => (PrecoVenda.HasValue && PrecoVenda.Value > 0)
            ? FormatHelper.FormatValor(PrecoVenda.Value)
            : string.Empty;

        public int? EstoqueAtual { get; set; }

        [NotMapped]
        [ValidateNever]
        public string EstoqueAtualFormatado => (EstoqueAtual.HasValue && EstoqueAtual.Value > 0)
            ? FormatHelper.FormatQuantidade((Int32)EstoqueAtual.Value) 
            : string.Empty;

        public int? EstoqueMinimo { get; set; }

        [NotMapped]
        [ValidateNever]
        public string EstoqueMinimoFormatado => (EstoqueMinimo.HasValue && EstoqueMinimo.Value > 0) 
            ? FormatHelper.FormatQuantidade((Int32)EstoqueMinimo.Value) 
            : string.Empty;

        [DataType(DataType.Date)]
        public DateTime DataUltimaCompra { get; set; }
        
        public string? Garantia { get; set; }
        
        public byte[]? Imagem { get; set; }

        // 🔹 Propriedades de navegação
        [ValidateNever] public virtual Fornecedor? Fornecedor { get; set; }
        
        [ValidateNever] public virtual Marca? Marca { get; set; }
        
        [ValidateNever] public virtual Modelo? Modelo { get; set; }
        
        [ValidateNever] public virtual Unidade? Unidade { get; set; }

        // Campos auxiliares
        public string? NomeFornecedor { get; set; }
        
        public string? NomeMarca { get; set; }
        
        public string? NomeModelo { get; set; }
        
        public string? NomeUnidade { get; set; }
    }
}