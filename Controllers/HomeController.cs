using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
using Ordem_Servicos_Web.Controllers.Cadastros;
using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Helpers;
using Ordem_Servicos_Web.Models;
using Ordem_Servicos_Web.Services;
=======
using Ordem_Servicos_Web.Models;
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
using System.Diagnostics;

namespace Ordem_Servicos_Web.Controllers
{
<<<<<<< HEAD

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
=======
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult ErroBanco()
        {
            return View();
        }

>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
