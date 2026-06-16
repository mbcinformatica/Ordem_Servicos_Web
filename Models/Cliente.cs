using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordem_Servicos_Web.Models
{
    [Table("DBClientes")]
    public class Cliente : PessoaFisicaJuridica
    {
        // Propriedade para o ID do cliente, que é a chave primária
        [Key]
        [Column("IDCliente")]
        public int IdCliente { get; set; }
    }
}
