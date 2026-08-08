using Microsoft.AspNetCore.Mvc;
using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Helpers;
using Ordem_Servicos_Web.Services;

namespace Ordem_Servicos_Web.Controllers.Login
{
    public class AccountController(MeuDbContext context, ILogger<AccountController> logger, LogService logService) : Controller
    {
        private readonly MeuDbContext _context = context;
        private readonly ILogger<AccountController> _logger = logger;
        private readonly LogService _logService = logService;

        // Exibir a página de login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Processar o login do usuário
        [HttpPost]
        public IActionResult Login(string login)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    // Busca o usuário pelo login com comparação case-sensitive
                    var usuario = _context.Usuarios
                    .FirstOrDefault(u => u.Login != null && u.Login.Equals(login, StringComparison.Ordinal));

                    HttpContext.Session.SetString("IdUsuario", usuario?.IdUsuario.ToString() ?? string.Empty);
                    var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
                    _logService.Registrar(idUsuario, "Login", "Account", 0, usuario?.NomeUsuario, "Login Efetuado com Sucesso!");
                    return RedirectToAction("Index", "Home");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao efetuar login");
            }
            TempData["Mensagem"] = "Erro ao Efetuar Login no Banco de Dados.";
            TempData["MensagemTipo"] = "erro";
            return View();
        }

        // Logout do usuário
        public IActionResult Logout()
        {
            // 🔹 Limpa toda a sessão (IdUsuario qualquer outro dado)
            HttpContext.Session.Clear();

            // Redireciona o usuário para a página de login
            return RedirectToAction("Login", "Account");
        }

        // Página de acesso negado
        [HttpGet]
        public IActionResult AcessoNegado()
        {
            return View();
        }
    }
}