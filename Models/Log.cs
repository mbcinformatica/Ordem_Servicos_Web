using System;
<<<<<<< HEAD
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ordem_Servicos_Web.Models
{
    [Table("Logs")]
    public class Log
    {
        // Chave primária
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        // Timestamp of when the log entry was created
        public DateTime Timestamp { get; set; }

        // Log level (e.g., Information, Warning, Error)
        public string Level { get; set; } = string.Empty;

        // The log message
        public string Message { get; set; } = string.Empty;

        // The exception details, if any
        public string? Exception { get; set; }

        // Additional properties or context for the log entry
        public string? Properties { get; set; }

        // User ID associated with the log entry (if applicable)
        [Column("IDUsuario")]
        public int IdUsuario { get; set; }

        // Action performed (e.g., Create, Update, Delete)
        public string Acao { get; set; } = string.Empty;

        // The name of the table affected by the action 
        public string Tabela { get; set; } = string.Empty;

        // The ID of the record affected by the action (if applicable)
        [Column("IDRegistro")]
        public int? IdRegistro { get; set; }

        // Additional observation or notes about the log entry
        public string? Observacao { get; set; }

        // Navigation property to the associated user (if applicable)
        [ForeignKey("IdUsuario")]
        public virtual Usuario? Usuario { get; set; }


=======

namespace Ordem_Servicos_Web.Models
{
    public class Log
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Exception { get; set; }
        public string? Properties { get; set; }
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
    }
}