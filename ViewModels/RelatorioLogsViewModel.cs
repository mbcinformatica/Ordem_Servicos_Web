using Ordem_Servicos_Web.Models;

namespace Ordem_Servicos_Web.ViewModels
{
    public class RelatorioLogsViewModel
    {
        public DateTime? Inicio { get; set; }
        public DateTime? Fim { get; set; }
        public string Usuario { get; set; } = string.Empty;
        public string Acao { get; set; } = string.Empty;

        public IEnumerable<Log> Logs { get; set; } = Enumerable.Empty<Log>();
    }
}
