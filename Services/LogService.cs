using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Models;
using System.Text.Json;

namespace Ordem_Servicos_Web.Services
{
    public class LogService
    {
        private readonly MeuDbContext _context;

        public LogService(MeuDbContext context)
        {
            _context = context;

        }

        public void Registrar(int idUsuario, string acao, string tabela, int? idRegistro, string? observacao, string level = "Information")
        {
            // Monta mensagem principal
            var mensagem = $"{acao} Realizado na Tabela {tabela} no Registro {idRegistro}";

            // Monta propriedades em JSON para facilitar auditoria
            var props = new
            {
                Usuario = idUsuario,
                Tabela = tabela,
                Registro = idRegistro,
                Acao = acao
            };

            var log = new Log
            {
                Timestamp = DateTime.UtcNow,
                Level = level,
                Message = mensagem,
                Properties = JsonSerializer.Serialize(props),
                IdUsuario = idUsuario,
                Tabela = tabela,
                IdRegistro = idRegistro,
                Acao = acao,
                Observacao = observacao
            };
            
            _context.Logs.Add(log);
            _context.SaveChanges();            
        }
    }
}
