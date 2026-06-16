using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ordem_Servicos_Web.Controllers.Cadastros;
using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Helpers;
using Ordem_Servicos_Web.Models;
using Ordem_Servicos_Web.Services;
using Ordem_Servicos_Web.Services.Interfaces;
using Serilog.Core;

namespace Ordem_Servicos_Web.Controllers.Ferramentas
{
    public class ImportarClientesController(MeuDbContext context, ILogger<ImportarClientesController> logger, ICnpjService<Cliente> cnpjService, PermissaoService permissaoService) : Controller
    {
        private readonly MeuDbContext _context = context;
        private readonly ILogger<ImportarClientesController> _logger = logger;
        private readonly PermissaoService _permissaoService = permissaoService;
        private readonly ICnpjService<Cliente> _cnpjService = cnpjService;
        public static int ProgressoImportacao { get; set; } = 0;

        [HttpGet]
        public IActionResult ImportarClientes()
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeExecutar(idUsuario, "FERRAMENTAS", "IMPORTAR-CLIENTES"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Acessar essa Tela.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ImportarClientes(IFormFile arquivoCSV)
        {
            if (arquivoCSV == null || arquivoCSV.Length == 0)
            {
                TempData["Mensagem"] = "Nenhum arquivo selecionado.";
                TempData["MensagemTipo"] = "erro";
                return RedirectToAction("ImportarClientes");
            }

            int totalImportados = 0;
            int totalDuplicados = 0;
            int totalInvalidos = 0;
            int totalLinhas = 0;
            using (var reader = new StreamReader(arquivoCSV.OpenReadStream()))
            {
                while (!reader.EndOfStream)
                {
                    var linha = await reader.ReadLineAsync();
                    if (!string.IsNullOrWhiteSpace(linha))
                        totalLinhas++;
                }
            }
            int linhaAtual = 0;
            using (var reader = new StreamReader(arquivoCSV.OpenReadStream()))
            {
                while (!reader.EndOfStream)
                {
                    var linha = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(linha)) continue;

                    linhaAtual++;
                    ProgressoImportacao = (int)((linhaAtual / (double)totalLinhas) * 100);

                    try
                    {
                        string cnpj = new string(linha.Where(char.IsDigit).ToArray());

                        if (!DocumentoHelper.ValidaCnpj(cnpj))
                        {
                            totalInvalidos++;
                            _logger.LogWarning($"CNPJ inválido ignorado: {cnpj}");
                            continue;
                        }

                        if (_context.Clientes.Any(c => c.CpfCnpj == cnpj))
                        {
                            totalDuplicados++;
                            _logger.LogInformation($"CNPJ duplicado ignorado: {cnpj}");
                            continue;
                        }

                        var cliente = await _cnpjService.ConsultarCnpjAsync(cnpj);
                        if (cliente == null)
                        {
                            totalInvalidos++;
                            _logger.LogWarning($"CNPJ não encontrado na API: {cnpj}");
                            continue;
                        }

                        cliente.CpfCnpj = FormatHelper.SomenteNumeros(cliente.CpfCnpj);
                        cliente.Cep = FormatHelper.SomenteNumeros(cliente.Cep);
                        cliente.FoneFixo = FormatHelper.SomenteNumeros(cliente.FoneFixo);
                        cliente.FoneCelular = FormatHelper.SomenteNumeros(cliente.FoneCelular);

                        _context.Clientes.Add(cliente);
                        _context.SaveChanges();
                        totalImportados++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Erro ao importar CNPJ da linha: {linha}");
                        TempData["Mensagem"] = "Erro ao inserir cliente: " + ex.Message;
                        TempData["MensagemTipo"] = "erro";
                        break;
                    }
                }
            }
            ProgressoImportacao = 100; // finalizado
            TempData["Mensagem"] = $"Importação concluída! {totalImportados} registros inseridos. {totalDuplicados} duplicados ignorados. {totalInvalidos} inválidos.";
            TempData["MensagemTipo"] = "sucesso";
            return RedirectToAction("ImportarClientes");
        }

        [HttpGet]
        public JsonResult Progresso()
        {
            return Json(new { progresso = ProgressoImportacao });
        }
    }
}
