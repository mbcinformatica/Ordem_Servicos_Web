<<<<<<< HEAD
﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Ordem_Servicos_Web.Helpers;
using System.ComponentModel.DataAnnotations;
=======
﻿using System.ComponentModel.DataAnnotations;
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordem_Servicos_Web.Models
{
    [Table("DBServicos")]
    public class Servico
    {
        [Key]
        [Column("IDServico")]
        public int IdServico { get; set; }

<<<<<<< HEAD
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
=======
        [Required, StringLength(20)]
        public string IdCodigoBase { get; set; }

        [Required]
        public int IdCategoriaServico { get; set; }

        [Required, StringLength(100)]
        public string Descricao { get; set; }

        public decimal? ValorServico { get; set; }
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
    }
}
