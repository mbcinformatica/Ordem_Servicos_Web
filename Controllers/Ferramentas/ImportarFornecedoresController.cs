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
                return RedirectToAction("ImportarFornecedores");
            }

            int totalImportados = 0;
            int totalDuplicados = 0;
            int totalInvalidos = 0;
            int totalLinhas = 0;

            // Primeiro loop: contar linhas válidas
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

            // Segundo loop: processar cada linha
            using (var reader = new StreamReader(arquivoCSV.OpenReadStream()))
            {
                string? linha;
                while ((linha = await reader.ReadLineAsync()) != null)
                {
                    if (string.IsNullOrWhiteSpace(linha)) continue;

                    linhaAtual++;
                    ProgressoImportacao = (int)((linhaAtual / (double)totalLinhas) * 100);

                    try
                    {
                        string cnpj = new([.. linha.Where(char.IsDigit)]);

                        if (!DocumentoHelper.ValidaCnpj(cnpj))
                        {
                            totalInvalidos++;
                            _logger.LogWarning($"CNPJ inválido ignorado: {cnpj}");
                            continue;
                        }

                        if (_context.Fornecedores.Any(f => f.CpfCnpj == cnpj))
                        {
                            totalDuplicados++;
                            _logger.LogInformation($"CNPJ duplicado ignorado: {cnpj}");
                            continue;
                        }

                        var fornecedor = await _cnpjService.ConsultarCnpjAsync(cnpj);
                        if (fornecedor == null)
                        {
                            totalInvalidos++;
                            _logger.LogWarning($"CNPJ não encontrado na API: {cnpj}");
                            continue;
                        }

                        fornecedor.CpfCnpj = FormatHelper.SomenteNumeros(fornecedor.CpfCnpj);
                        fornecedor.Cep = FormatHelper.SomenteNumeros(fornecedor.Cep);
                        fornecedor.FoneFixo = FormatHelper.SomenteNumeros(fornecedor.FoneFixo);
                        fornecedor.FoneCelular = FormatHelper.SomenteNumeros(fornecedor.FoneCelular);

                        _context.Fornecedores.Add(fornecedor);
                        await _context.SaveChangesAsync(); // 🔹 use versão assíncrona
                        totalImportados++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Erro ao importar CNPJ da linha: {linha}");
                        TempData["Mensagem"] = "Erro ao inserir fornecedor: " + ex.Message;
                        TempData["MensagemTipo"] = "erro";
                        break;
                    }
                }
            }

            ProgressoImportacao = 100; // finalizado
            TempData["Mensagem"] = $"Importação concluída! {totalImportados} registros inseridos. {totalDuplicados} duplicados ignorados. {totalInvalidos} inválidos.";
            TempData["MensagemTipo"] = "sucesso";
            return RedirectToAction("ImportarFornecedores");
        }

        [HttpGet]
        public JsonResult Progresso()
        {
            return Json(new { progresso = ProgressoImportacao });
        }
    }
}
