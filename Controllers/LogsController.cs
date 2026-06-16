using Microsoft.AspNetCore.Mvc;
using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Models;
using System.Linq;

namespace Ordem_Servicos_Web.Controllers
{
    public class LogsController : Controller
    {
        private readonly MeuDbContext _context;

        public LogsController(MeuDbContext context)
        {
            _context = context;
        }

        public IActionResult Dashboard()
        {
            var vm = new LogsDashboardViewModel
            {
                UltimosLogs = _context.Logs
                    .OrderByDescending(l => l.Timestamp)
                    .Take(10)
                    .Select(l => new LogEntry
                    {
                        Timestamp = l.Timestamp,
                        Level = l.Level,
                        Message = l.Message,
                        Exception = l.Exception
                    }).ToList(),

                ErrosCriticos24h = _context.Logs
                    .Where(l => l.Level == "Error" && l.Timestamp >= DateTime.Now.AddDays(-1))
                    .OrderByDescending(l => l.Timestamp)
                    .Select(l => new LogEntry
                    {
                        Timestamp = l.Timestamp,
                        Level = l.Level,
                        Message = l.Message,
                        Exception = l.Exception
                    }).ToList(),

                QuantidadePorNivel = _context.Logs
                    .GroupBy(l => l.Level)
                    .Select(g => new NivelCount
                    {
                        Level = g.Key,
                        Total = g.Count()
                    }).ToList(),

                QuantidadePorDia = _context.Logs
                    .GroupBy(l => l.Timestamp.Date)
                    .Select(g => new DiaCount
                    {
                        Dia = g.Key,
                        Total = g.Count()
                    })
                    .OrderByDescending(g => g.Dia)
                    .ToList(),
            };

            return View(vm);
        }
    }
}
