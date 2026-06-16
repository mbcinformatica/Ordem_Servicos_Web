using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Models;
using System.Linq;

namespace Ordem_Servicos_Web.Controllers.Consultas
{
    public class LogsController(MeuDbContext context) : Controller
    {
        private readonly MeuDbContext _context = context;

        public IActionResult Dashboard()
        {
            var vm = new LogsDashboardViewModel
            {
                UltimosLogs = [.. _context.Logs
                    .OrderByDescending(l => l.Timestamp)
                    .Take(10)
                    .Select(l => new LogEntry
                    {
                        Timestamp = l.Timestamp,
                        Level = l.Level,
                        Message = l.Message,
                        Exception = l.Exception
                    })],

                ErrosCriticos24h = [.. _context.Logs
                    .Where(l => l.Level == "Error" && l.Timestamp >= DateTime.Now.AddDays(-1))
                    .OrderByDescending(l => l.Timestamp)
                    .Select(l => new LogEntry
                    {
                        Timestamp = l.Timestamp,
                        Level = l.Level,
                        Message = l.Message,
                        Exception = l.Exception
                    })],

                QuantidadePorNivel = [.. _context.Logs
                    .GroupBy(l => l.Level)
                    .Select(g => new NivelCount
                    {
                        Level = g.Key,
                        Total = g.Count()
                    })],

                QuantidadePorDia = [.. _context.Logs
                    .GroupBy(l => l.Timestamp.Date)
                    .Select(g => new DiaCount
                    {
                        Dia = g.Key,
                        Total = g.Count()
                    })
                    .OrderByDescending(g => g.Dia)],
            };

            return View(vm);
        }

        public IActionResult AuditoriaLog(
            int page = 1,
            DateTime? inicio = null,
            DateTime? fim = null,
            int? usuario = null,
            string? acao = "",
            string search = "",
            string column = "Timestamp")
        {
            int pageSize = 10;

            var query = _context.Logs
                .Include(l => l.Usuario) // só se houver relacionamento
                .AsQueryable();

            // Filtros opcionais
            if (inicio.HasValue)
                query = query.Where(l => l.Timestamp >= inicio.Value);

            if (fim.HasValue)
                query = query.Where(l => l.Timestamp <= fim.Value);

            if (usuario.HasValue)
                query = query.Where(l => l.IdUsuario == usuario.Value); // usa FK simples

            if (!string.IsNullOrWhiteSpace(acao))
                query = query.Where(l => l.Acao == acao);

            if (!string.IsNullOrWhiteSpace(search))
                query = ApplySearchFilter(query, search, column);

            query = ApplyOrdering(query, column);

            var logs = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            int totalRegistros = query.Count();
            int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)pageSize);

            ViewBag.Page = page;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.Search = search;
            ViewBag.Column = column;

            return View(logs);
        }

        // Action para pesquisa Ajax
        public IActionResult Search(DateTime? inicio = null, DateTime? fim = null, int? usuario = null, string? acao = "", string search = "", string column = "Timestamp")
        {
            var query = _context.Logs.Include(l => l.Usuario).AsQueryable();

            if (inicio.HasValue)
                query = query.Where(l => l.Timestamp >= inicio.Value);

            if (fim.HasValue)
                query = query.Where(l => l.Timestamp <= fim.Value);

            if (usuario.HasValue)
                query = query.Where(l => l.IdUsuario == usuario.Value);

            if (!string.IsNullOrEmpty(acao))
                query = query.Where(l => l.Acao == acao);

            if (!string.IsNullOrEmpty(search))
                query = ApplySearchFilter(query, search, column);

            query = ApplyOrdering(query, column);

            var logs = query.OrderByDescending(l => l.Timestamp).Take(100).ToList();

            return PartialView("_LogsTable", logs);
        }

        // Método auxiliar para aplicar filtro
        private static IQueryable<Log> ApplySearchFilter(IQueryable<Log> query, string search, string column)
        {
            switch (column)
            {
                case "Id":
                    if (int.TryParse(search, out int id))
                        query = query.Where(lg => lg.Id == id);
                    break;
                case "Usuario":
                    query = query.Where(lg => lg.Usuario != null && lg.Usuario.NomeUsuario.StartsWith(search));
                    break;
                case "Acao":
                    query = query.Where(lg => lg.Acao.StartsWith(search));
                    break;
                default: // Mensagem
                    query = query.Where(lg => lg.Message.StartsWith(search));
                    break;
            }
            return query;
        }

        // Método auxiliar para aplicar ordenação
        private static IQueryable<Log> ApplyOrdering(IQueryable<Log> query, string column)
        {
            return column switch
            {
                "Id" => query.OrderBy(lg => lg.Id),
                "IdUsuario" => query.OrderBy(lg => lg.IdUsuario),
                "Acao" => query.OrderBy(lg => lg.Acao),
                _ => query.OrderByDescending(lg => lg.Timestamp),
            };
        }

        public JsonResult GetUsuarios()
        {
            var usuarios = _context.Usuarios
                .Select(u => new { u.IdUsuario, u.NomeUsuario })
                .ToList();
            return Json(usuarios);
        }
    }
}
