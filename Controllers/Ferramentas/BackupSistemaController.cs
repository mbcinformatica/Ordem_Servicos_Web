using Microsoft.AspNetCore.Mvc;
using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Helpers;
using Ordem_Servicos_Web.Services;

namespace Ordem_Servicos_Web.Controllers.Ferramentas
{
    public class BackupSistemaController(
        MySqlBackupService backupService,
        MeuDbContext context,
        ILogger<BackupSistemaController> logger,
        PermissaoService permissaoService,
        LogService logService) : Controller
    {
        private readonly MySqlBackupService _backupService = backupService;
        private readonly MeuDbContext _context = context;
        private readonly ILogger<BackupSistemaController> _logger = logger;
        private readonly PermissaoService _permissaoService = permissaoService;
        private readonly LogService _logService = logService;

        [HttpGet]
        public IActionResult BackupSistema()
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);

            if (!_permissaoService.PodeExecutar(idUsuario, "FERRAMENTAS", "BACKUP"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para acessar essa tela.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost, ActionName("BackupSistema")]
        public async Task<IActionResult> ConfirmadoBackupSistema()
        {
            try
            {

                var arquivos = await _backupService.BackupTablesAsync(new List<string>
                {
                    "DBCategoriaServicos",
                    "DBClientes",
                    "DBFornecedores",
                    "DBLancamentoServicos",
                    "DBMarcas",
                    "DBModelos",
                    "DBProdutos",
                    "DBServicos",
                    "DBUnidades",
                    "DBUsuarios",
                    "DBItensMenu",
                    "DBPermissoes",
                    "DBMenu"
                });
                
                var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
                _logService.Registrar(idUsuario, "Backup", "BancoDados", 0, null, "Backup Concluído com Sucesso!");

                TempData["Mensagem"] = "Backup Concluído com Sucesso!";
                TempData["MensagemTipo"] = "sucesso";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao Realizar Backup do bBanco de Dados.");
                TempData["Mensagem"] = "Erro ao Realizar Backup:";
                TempData["MensagemTipo"] = "erro";
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
