using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordem_Servicos_Web.Data;
using System.Security.Claims;

namespace Ordem_Servicos_Web.Controllers.Login
{
    public class AccountController(MeuDbContext context, ILogger<AccountController> logger) : Controller
    {
        private readonly MeuDbContext _context = context;
        private readonly ILogger<AccountController> _logger = logger;

        // Exibir a página de login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Processar o login do usuário
        [HttpPost]
        public IActionResult Login(string login, string senha)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    // Busca o usuário pelo login com comparação case-sensitive
                    var usuario = _context.Usuarios
                    .FirstOrDefault(u => u.Login.Equals(login, StringComparison.Ordinal));

                    HttpContext.Session.SetString("IdUsuario", usuario.IdUsuario.ToString());
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