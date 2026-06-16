using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
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


        [ModelBinder(BinderType = typeof(DecimalModelBinder))]
        public decimal PrecoCompra { get; set; }

        // Propriedade para exibir o valor formatado
        [NotMapped]
        [ValidateNever]
        public string PrecoCompraFormatado => PrecoCompra > 0
            ? FormatHelper.FormatValor(PrecoCompra)
            : string.Empty;

        [ModelBinder(BinderType = typeof(DecimalModelBinder))]
        public decimal PrecoVenda { get; set; }

        // Propriedade para exibir o valor formatado
        [NotMapped]
        [ValidateNever]
        public string PrecoVendaFormatado => PrecoVenda > 0
            ? FormatHelper.FormatValor(PrecoVenda)
            : string.Empty;

        [ModelBinder(BinderType = typeof(DecimalModelBinder))]
        public int EstoqueAtual { get; set; }

        // Propriedade para exibir o valor formatado
        [NotMapped]
        [ValidateNever]
        public string EstoqueAtualFormatado => EstoqueAtual > 0
            ? FormatHelper.FormatQuantidade(EstoqueAtual)
            : string.Empty;

        [ModelBinder(BinderType = typeof(DecimalModelBinder))]
        public int EstoqueMinimo { get; set; }

        // Propriedade para exibir o valor formatado
        [NotMapped]
        [ValidateNever]
        public string EstoqueMinimoFormatado => EstoqueMinimo > 0
            ? FormatHelper.FormatQuantidade(EstoqueMinimo)
            : string.Empty;

        public DateTime DataUltimaCompra { get; set; }

        public string? Garantia { get; set; }

        public byte[]? Imagem { get; set; }

        // Propriedades de navegação
        [ValidateNever]
        public virtual Fornecedor? Fornecedor { get; set; }

        // Propriedades de navegação
        [ValidateNever]
        public virtual Marca? Marca { get; set; }
        // Propriedades de navegação
        [ValidateNever]
        public virtual Modelo? Modelo { get; set; }

        // Propriedades de navegação
        [ValidateNever]
        public virtual Unidade? Unidade { get; set; }

        // Campos auxiliares para exibição (somente leitura)
        public string? NomeFornecedor { get; set; }
        public string? NomeMarca { get; set; }
        public string? NomeModelo { get; set; }
        public string? NomeUnidade { get; set; }
    }
}