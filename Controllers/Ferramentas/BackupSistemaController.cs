using Microsoft.AspNetCore.Mvc;
using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Helpers;
using Ordem_Servicos_Web.Services;

namespace Ordem_Servicos_Web.Controllers.Ferramentas
{
    public class BackupSistemaController : Controller
    {
        private readonly MySqlBackupService _backupService;
        private readonly MeuDbContext _context;
        private readonly ILogger<BackupSistemaController> _logger;
        private readonly PermissaoService _permissaoService;

        public BackupSistemaController(
            MySqlBackupService backupService,
            MeuDbContext context,
            ILogger<BackupSistemaController> logger,
            PermissaoService permissaoService)
        {
            _backupService = backupService;
            _context = context;
            _logger = logger;
            _permissaoService = permissaoService;
        }

        [HttpGet]
        public IActionResult BackupSistema()
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);

            if (!_permissaoService.PodeExecutar(idUsuario, "FERRAMENTAS", "BACKUP"))
            {
                TempData["Mensagem"] = "Você não tem permissão para acessar essa tela.";
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

                TempData["Mensagem"] = "Backup Concluído com Sucesso!";
                TempData["MensagemTipo"] = "sucesso";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar backup do banco de dados.");
                TempData["Mensagem"] = "Erro ao Realizar Backup:";
                TempData["MensagemTipo"] = "erro";
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
