using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Ordem_Servicos_Web.Attributes;
using Ordem_Servicos_Web.Helpers;
using Ordem_Servicos_Web.Models;
using System;
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

        public decimal? PrecoCompra { get; set; }

        [NotMapped]
        [ValidateNever]
        public string PrecoCompraFormatado => PrecoCompra > 0 
            ? FormatHelper.FormatValor(PrecoCompra.Value) 
            : string.Empty;

        public decimal? PrecoVenda { get; set; }

        [NotMapped]
        [ValidateNever]
        public string PrecoVendaFormatado => PrecoVenda > 0 
            ? FormatHelper.FormatValor(PrecoVenda.Value) 
            : string.Empty;

        public int EstoqueAtual { get; set; }

        [NotMapped]
        [ValidateNever]
        public string EstoqueAtualFormatado =>
            EstoqueAtual > 0 ? FormatHelper.FormatQuantidade(EstoqueAtual) : string.Empty;

        public int EstoqueMinimo { get; set; }

        [NotMapped]
        [ValidateNever]
        public string EstoqueMinimoFormatado =>
            EstoqueMinimo > 0 ? FormatHelper.FormatQuantidade(EstoqueMinimo) : string.Empty;

        [DataType(DataType.Date)]
        public DateTime DataUltimaCompra { get; set; }

        public string? Garantia { get; set; }

        public byte[]? Imagem { get; set; }

        [ValidateNever]
        public virtual Fornecedor? Fornecedor { get; set; }

        [ValidateNever]
        public virtual Marca? Marca { get; set; }

        [ValidateNever]
        public virtual Modelo? Modelo { get; set; }

        [ValidateNever]
        public virtual Unidade? Unidade { get; set; }

        // Campos auxiliares
        public string? NomeFornecedor { get; set; }
        public string? NomeMarca { get; set; }
        public string? NomeModelo { get; set; }
        public string? NomeUnidade { get; set; }
    }
}