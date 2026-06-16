using System;
using System.Collections.Generic;

namespace Ordem_Servicos_Web.Models
{
    public class LogEntry
    {
<<<<<<< HEAD
        // Propriedades comuns de um log
        public DateTime Timestamp { get; set; }

        // Nível do log (e.g., Information, Warning, Error)
        public string Level { get; set; } = string.Empty;

        // Mensagem do log
        public string Message { get; set; } = string.Empty;

        // Exceção associada ao log, se houver
=======
        public DateTime Timestamp { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
        public string? Exception { get; set; }
    }

    // Classe auxiliar para agregação por nível
    public class NivelCount
    {
<<<<<<< HEAD
        // Nível do log (e.g., Information, Warning, Error)
        public string Level { get; set; } = string.Empty;

        // Quantidade de logs para esse nível
=======
        public string Level { get; set; } = string.Empty;
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
        public int Total { get; set; }
    }

    // Classe auxiliar para agregação por dia
    public class DiaCount
    {
<<<<<<< HEAD
        // Data do dia
        public DateTime Dia { get; set; }

        // Quantidade de logs para esse dia
=======
        public DateTime Dia { get; set; }
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
        public int Total { get; set; }
    }

    public class LogsDashboardViewModel
    {
<<<<<<< HEAD
        // Lista dos últimos logs
        public List<LogEntry> UltimosLogs { get; set; } = new();

        // Lista dos erros críticos das últimas 24 horas
        public List<LogEntry> ErrosCriticos24h { get; set; } = new();

        // Quantidade de logs por nível
        public List<NivelCount> QuantidadePorNivel { get; set; } = new();

        // Quantidade de logs por dia
=======
        public List<LogEntry> UltimosLogs { get; set; } = new();
        public List<LogEntry> ErrosCriticos24h { get; set; } = new();
        public List<NivelCount> QuantidadePorNivel { get; set; } = new();
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
        public List<DiaCount> QuantidadePorDia { get; set; } = new();
    }
}
