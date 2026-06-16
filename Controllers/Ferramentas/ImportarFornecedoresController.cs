using Microsoft.AspNetCore.Mvc;
using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Helpers;
using Ordem_Servicos_Web.Models;
using Ordem_Servicos_Web.Services;
using Ordem_Servicos_Web.Services.Interfaces;

namespace Ordem_Servicos_Web.Controllers.Ferramentas
{
    public class ImportarFornecedoresController(
        MeuDbContext context,
        ILogger<ImportarFornecedoresController> logger,
        ICnpjService<Fornecedor> cnpjService,
        PermissaoService permissaoService) : Controller
    {
        private readonly MeuDbContext _context = context;
        private readonly ILogger<ImportarFornecedoresController> _logger = logger;
        private readonly ICnpjService<Fornecedor> _cnpjService = cnpjService; // 🔹 serviço tipado para Fornecedor
        private readonly PermissaoService _permissaoService = permissaoService;
        public static int ProgressoImportacao { get; set; } = 0;

        [HttpGet]
        public IActionResult ImportarFornecedores()
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeExecutar(idUsuario, "FERRAMENTAS", "IMPORTAR-FORNECEDORES"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Acessar essa Tela.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ImportarFornecedores(IFormFile arquivoCSV)
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
                string? linha;
                while ((linha = await reader.ReadLineAsync()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(linha))
                        totalLinhas++;
                }
            }
            int linhaAtual = 0;
            using (var reader = new StreamReader(arquivoCSV.OpenReadStream()))
            {
                string? linha;
                while ((linha = await reader.ReadLineAsync()) != null)
                {
                    if (string.IsNullOrWhiteSpace(linha)) continue; if (string.IsNullOrWhiteSpace(linha)) continue;

                    linhaAtual++;
                    ProgressoImportacao = (int)((linhaAtual / (double)totalLinhas) * 100);

                    try
                    {
                        string cnpj = string.Concat(linha.Where(char.IsDigit));

                        if (!DocumentoHelper.ValidaCnpj(cnpj))
                        {
                            totalInvalidos++;
                            if (_logger.IsEnabled(LogLevel.Warning))
                            {
                                _logger.LogWarning("CNPJ inválido ignorado: {Cnpj}", cnpj);
                            }
                            continue;
                        }

                        if (_context.Fornecedores.Any(c => c.CpfCnpj == cnpj))
                        {
                            totalDuplicados++;
                            if (_logger.IsEnabled(LogLevel.Information))
                            {
                                _logger.LogInformation("CNPJ duplicado ignorado: {Cnpj}", cnpj);
                            }
                            continue;
                        }

                        var fornecedor = await _cnpjService.ConsultarCnpjAsync(cnpj);
                        if (fornecedor == null)
                        {
                            totalInvalidos++;
                            if (_logger.IsEnabled(LogLevel.Warning))
                            {
                                _logger.LogWarning("CNPJ não encontrado na API: {Cnpj}", cnpj);
                            }
                            continue;
                        }

                        fornecedor.CpfCnpj = FormatHelper.SomenteNumeros(fornecedor.CpfCnpj);
                        fornecedor.Cep = FormatHelper.SomenteNumeros(fornecedor.Cep);
                        fornecedor.FoneFixo = FormatHelper.SomenteNumeros(fornecedor.FoneFixo);
                        fornecedor.FoneCelular = FormatHelper.SomenteNumeros(fornecedor.FoneCelular);

                        _context.Fornecedores.Add(fornecedor);
                        _context.SaveChanges();
                        totalImportados++;
                    }
                    catch (Exception ex)
                    {
                        if (_logger.IsEnabled(LogLevel.Error))
                        {
                            _logger.LogError(ex, "Erro ao importar CNPJ da linha: {Linha}", linha);
                        }
                        TempData["Mensagem"] = "Erro ao Inserir Fornecedor: " + ex.Message;
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
