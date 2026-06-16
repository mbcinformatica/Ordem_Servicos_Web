using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Ordem_Servicos_Web.Helpers;
using Ordem_Servicos_Web.Services;
using System.Text;

namespace Ordem_Servicos_Web.Controllers.Ferramentas
{
    public class RestoreBackupController(
        IConfiguration config,
        ILogger<RestoreBackupController> logger,
        PermissaoService permissaoService) : Controller
    {
        private readonly string _connectionString = config.GetConnectionString("DefaultConnection") ?? string.Empty;
        private readonly ILogger<RestoreBackupController> _logger = logger;
        private readonly PermissaoService _permissaoService = permissaoService;

        [HttpGet]
        public IActionResult RestoreBackup()
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);

            if (!_permissaoService.PodeExecutar(idUsuario, "FERRAMENTAS", "RESTAURE"))
            {
                TempData["Mensagem"] = "Você não tem permissão para acessar essa tela.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RestoreBackup(IFormFile arquivoSQL)
        {
            if (arquivoSQL == null || arquivoSQL.Length == 0)
            {
                TempData["Mensagem"] = "Nenhum arquivo selecionado.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("RestoreBackup");
            }

            string tableName = "";
            try
            {
                // Lê conteúdo do arquivo .sql
                string dumpContent;
                using (var reader = new StreamReader(arquivoSQL.OpenReadStream(), Encoding.UTF8))
                {
                    dumpContent = await reader.ReadToEndAsync();
                }

                // Identifica a tabela e coleta os INSERTs
                var lines = dumpContent.Split('\n');
                var inserts = new List<string>();

                foreach (var line in lines)
                {
                    if (line.StartsWith("INSERT INTO"))
                    {
                        inserts.Add(line.Trim());
                        if (string.IsNullOrEmpty(tableName))
                        {
                            int start = line.IndexOf('`') + 1;
                            int end = line.IndexOf('`', start);
                            tableName = line[start..end];
                        }
                    }
                }

                if (inserts.Count == 0)
                {
                    TempData["Mensagem"] = "Nenhum dado encontrado no arquivo de backup.";
                    TempData["MensagemTipo"] = "aviso";
                    return RedirectToAction("RestoreBackup");
                }

                using (var conn = new MySqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    using var cmd = conn.CreateCommand();
                    // 🔹 Preparação da tabela: limpa dados e zera AUTO_INCREMENT
                    cmd.CommandText = $"SET FOREIGN_KEY_CHECKS=0; " +
                                      $"TRUNCATE TABLE `{tableName}`; " +
                                      $"ALTER TABLE `{tableName}` AUTO_INCREMENT = 1; " +
                                      $"SET FOREIGN_KEY_CHECKS=1;";
                    await cmd.ExecuteNonQueryAsync();

                    // 🔹 Executa os INSERTs
                    foreach (var insert in inserts)
                    {
                        cmd.CommandText = insert;
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                TempData["Mensagem"] = $"Backup da tabela {tableName} restaurado com sucesso!";
                TempData["MensagemTipo"] = "sucesso";
            }
            catch (Exception ex)
            {
                TempData["Mensagem"] = $"Erro ao restaurar backup: {ex.Message}";
                TempData["MensagemTipo"] = "erro";
            }

            return RedirectToAction("Index", "Home");
        }
    }
}