using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Event;
using iText.Layout.Properties;
using iText.Layout.Element;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Models;
using iText.Kernel.Geom;
using iText.Layout;

namespace Ordem_Servicos_Web.Controllers.Consultas
{
    public class LogsController(MeuDbContext context) : Controller
    {
        private readonly MeuDbContext _context = context;

        public IActionResult AuditoriaLog(
            int page = 1,
            DateTime? inicio = null,
            DateTime? fim = null,
            int? usuario = null,
            string? acao = "",
            string column = "Timestamp")
        {
            if (page < 1) page = 1;
            int pageSize = 100;

            var query = _context.Logs
                .Include(l => l.Usuario)
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
            ViewBag.Column = column;

            return View(logs);
        }
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

        public IActionResult LogsPdf(
            DateTime? inicio = null,
            DateTime? fim = null,
            int? usuario = null,
            string? acao = "",
            string column = "Timestamp")
        {

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

            query = ApplyOrdering(query, column);

            var logs = query.ToList();

            if (logs.Count == 0)
            {
                TempData["Mensagem"] = "Nenhum log encontrado.";
                TempData["MensagemTipo"] = "info";
                return RedirectToAction("AuditoriaLog");
            }

            using var ms = new MemoryStream();
            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            pdf.SetDefaultPageSize(PageSize.A4.Rotate());

            pdf.AddEventHandler(PdfDocumentEvent.START_PAGE, new PdfHeaderFooter("Relatório de Logs do Sistema"));

            using var document = new Document(pdf);
            document.SetMargins(50f, 20f, 30f, 20f);

            // Apenas 4 colunas: Data/Hora, Mensagem, Usuário, Ação
            float[] columnWidths = [20, 45, 20, 15];
            var table = new Table(UnitValue.CreatePercentArray(columnWidths)).UseAllAvailableWidth();

            string[] headers = ["Data/Hora", "Mensagem", "Usuário", "Ação"];
            var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

            foreach (var h in headers)
            {
                table.AddHeaderCell(new Cell()
                    .Add(new Paragraph(h).SetFont(boldFont).SetFontSize(9))
                    .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetPadding(3));
            }

            bool linhaPar = false;
            foreach (var log in logs)
            {
                var bgColor = linhaPar ? ColorConstants.LIGHT_GRAY : ColorConstants.WHITE;

                table.AddCell(new Cell().Add(new Paragraph(log.Timestamp.ToString("dd/MM/yyyy HH:mm:ss")).SetFontSize(8)).SetBackgroundColor(bgColor).SetPadding(2));
                table.AddCell(new Cell().Add(new Paragraph(log.Message ?? "-")).SetFontSize(8).SetBackgroundColor(bgColor).SetPadding(2));
                table.AddCell(new Cell().Add(new Paragraph(log.Usuario?.NomeUsuario ?? "-")).SetFontSize(8).SetBackgroundColor(bgColor).SetPadding(2));
                table.AddCell(new Cell().Add(new Paragraph(log.Acao ?? "-")).SetFontSize(8).SetBackgroundColor(bgColor).SetPadding(2));

                linhaPar = !linhaPar;
            }

            document.Add(table);
            document.Close();

            Response.Headers.Append("Content-Disposition", "inline; filename=RelatorioLogs.pdf");
            return File(ms.ToArray(), "application/pdf");
        }

    }
}