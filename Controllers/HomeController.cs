using Microsoft.AspNetCore.Mvc;
using Ordem_Servicos_Web.Controllers.Cadastros;
using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Helpers;
using Ordem_Servicos_Web.Models;
using Ordem_Servicos_Web.Services;
using System.Diagnostics;

namespace Ordem_Servicos_Web.Controllers
{

    public class HomeController(PermissaoService permissaoService) : Controller
    {
        private readonly PermissaoService _permissaoService = permissaoService;

        // Ação para a página inicial
        public IActionResult Index()
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (idUsuario == 0)
            {
                TempData["Mensagem"] = "Você não Esta Logado no Sistema. Favor Efetuar Login.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        // Ação para a página de erro de banco de dados
        public IActionResult ErroBanco()
        {
            // Recupera mensagem do filtro via TempData
            TempData["Mensagem"] = HttpContext.Items["Mensagem"]?.ToString()
                                   ?? "Não foi Possível Conectar ao Banco de Dados. Tente Novamente mais Tarde.";
            TempData["MensagemTipo"] = HttpContext.Items["MensagemTipo"]?.ToString() ?? "erro";

            return View();
        }

        // Ação para a página de erro genérico
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
