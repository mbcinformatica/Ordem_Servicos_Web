using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Helpers;
using Ordem_Servicos_Web.Models;
using Ordem_Servicos_Web.Services;

namespace Ordem_Servicos_Web.Controllers.Configuracoes
{
    public class PermissoesController(MeuDbContext context, ILogger<PermissoesController> logger, PermissaoService permissaoService) : Controller
    {
        private readonly MeuDbContext _context = context;
        private readonly ILogger<PermissoesController> _logger = logger;
        private readonly PermissaoService _permissaoService = permissaoService;

        // Index com paginação + pesquisa
        public IActionResult Index(int page = 1, string search = "", string column = "UsuarioNomeUsuario")
        {

            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeExecutar(idUsuario, "CONFIGURAÇÕES", "PERMISSÕES DE ACESSO"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Acessar essa Tela.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index", "Home");
            }

            int pageSize = 10;

            var query = _context.Permissoes
                .Include(p => p.Usuario)
                .Include(p => p.Menu)
                .Include(p => p.ItensMenu)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = ApplySearchFilter(query, search, column);
            }

            query = ApplyOrdering(query, column);

            var permissoes = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            int totalRegistros = query.Count();
            int totalPaginas = (int)Math.Ceiling(totalRegistros / (double)pageSize);

            ViewBag.Page = page;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.TotalRegistros = totalRegistros;
            ViewBag.Search = search;
            ViewBag.Column = column;

            return View(permissoes);
        }

        // Action para pesquisa Ajax
        public IActionResult Search(string search = "", string column = "UsuarioNomeUsuario")
        {
            var query = _context.Permissoes
                .Include(p => p.Usuario)
                .Include(p => p.Menu)
                .Include(p => p.ItensMenu)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = ApplySearchFilter(query, search, column);

            }

            query = ApplyOrdering(query, column);

            var permissoes = query.ToList();

            return PartialView("_PermissoesTable", permissoes);
        }

        // Método auxiliar para aplicar filtro
        private static IQueryable<Permissao> ApplySearchFilter(IQueryable<Permissao> query, string search, string column)
        {
            switch (column)
            {
                case "IdPermissao":
                    if (int.TryParse(search, out int id))
                        query = query.Where(mo => mo.IdPermissao == id);
                    break;
                case "MenuDescricao":
                    query = query.Where(mo => mo.Menu.Descricao.StartsWith(search));
                    break;
                case "ItensMenuDescricao":
                    query = query.Where(mo => mo.ItensMenu.Descricao.StartsWith(search));
                    break;
                default: // Descricao
                    query = query.Where(mo => mo.Usuario.NomeUsuario.StartsWith(search));
                    break;
            }
            return query;
        }

        // Método auxiliar para aplicar ordenação
        private static IQueryable<Permissao> ApplyOrdering(IQueryable<Permissao> query, string column)
        {
            return column switch
            {
                "IdPermissao" => query.OrderBy(mo => mo.IdPermissao),
                "IdMenu" => query.OrderBy(mo => mo.IdMenu),
                "IdItensMenu" => query.OrderBy(mo => mo.IdItensMenu),
                _ => query.OrderBy(mo => mo.IdUsuario),
            };
        }

        // GET: Permissoes/Create
        public IActionResult Create()
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeCriar(idUsuario, "CONFIGURAÇÕES", "PERMISSÕES DE ACESSO"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Executar essa Ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }

            return View(new Permissao());
        }

        // POST: Permissoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Permissao permissao)
        {
            try
            {
                // Remove validação das propriedades de navegação
                ModelState.Remove("Menu");
                ModelState.Remove("Usuario");
                ModelState.Remove("ItensMenu");

                if (ModelState.IsValid)
                {
                    _context.Permissoes.Add(permissao);
                    _context.SaveChanges();

                    TempData["Mensagem"] = "Permissão Incluída com Sucesso!";
                    TempData["MensagemTipo"] = "sucesso";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Mensagem"] = "Erro na Validação das Informações.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao Incluir Permissão.");
                TempData["Mensagem"] = "Erro ao Salvar no Banco de Dados.";
                TempData["MensagemTipo"] = "erro";
            }

            return View(permissao);
        }

        // GET: Permissoes/Alterar/5
        public IActionResult Alterar(int id)
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeAlterar(idUsuario, "CONFIGURAÇÕES", "PERMISSÕES DE ACESSO"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Executar essa Ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }

            var permissao = _context.Permissoes
                .Include(p => p.Usuario)
                .Include(p => p.Menu)
                .Include(p => p.ItensMenu)
                .FirstOrDefault(p => p.IdPermissao == id);

            if (permissao == null)
            {
                return NotFound();
            }

            return View(permissao);
        }

        // POST: Permissoes/Alterar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Alterar(Permissao permissao)
        {
            try
            {
                ModelState.Remove("Menu");
                ModelState.Remove("Usuario");
                ModelState.Remove("ItensMenu");

                var permissaoDb = _context.Permissoes
                    .FirstOrDefault(p => p.IdPermissao == permissao.IdPermissao);

                if (permissaoDb == null)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    permissaoDb.Executar = permissao.Executar;
                    permissaoDb.Criar = permissao.Criar;
                    permissaoDb.Alterar = permissao.Alterar;
                    permissaoDb.Excluir = permissao.Excluir;

                    _context.Permissoes.Update(permissaoDb);
                    _context.SaveChanges();

                    TempData["Mensagem"] = "Permissão Alterada com Sucesso!";
                    TempData["MensagemTipo"] = "sucesso";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Mensagem"] = "Erro na Validação das Informações.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao Alterar Permissão.");
                TempData["Mensagem"] = "Erro ao Salvar no Banco de Dados.";
                TempData["MensagemTipo"] = "erro";
            }
            return View(permissao);
        }

        // GET: Permissoes/Excluir/5
        public IActionResult Excluir(int id)
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeExcluir(idUsuario, "CONFIGURAÇÕES", "PERMISSÕES DE ACESSO"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Executar essa Ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }

            var permissao = _context.Permissoes
                .Include(p => p.Usuario)
                .Include(p => p.Menu)
                .Include(p => p.ItensMenu)
                .FirstOrDefault(p => p.IdPermissao == id);

            if (permissao == null)
            {
                return NotFound();
            }
            return View(permissao);
        }

        // POST: Permissoes/Excluir/5
        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public IActionResult ExcluirConfirmado(int id)
        {
            try
            {
                var permissao = _context.Permissoes.Find(id);
                if (permissao != null)
                {
                    _context.Permissoes.Remove(permissao);
                    _context.SaveChanges();
                    TempData["Mensagem"] = "Permissão Excluída com Sucesso!";
                    TempData["MensagemTipo"] = "sucesso";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao Excluir Permissão.");
                TempData["Mensagem"] = "Erro ao Excluir no Banco de Dados.";
                TempData["MensagemTipo"] = "erro";
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult GetUsuarios()
        {
            var usuarios = _context.Usuarios
                .Select(u => new { u.IdUsuario, u.NomeUsuario })
                .ToList();

            return Json(usuarios);
        }

        [HttpGet]
        public JsonResult GetMenus()
        {
            var menus = _context.Menus
                .Select(m => new { m.IdMenu, m.Descricao })
                .ToList();

            return Json(menus);
        }

        [HttpGet]
        public JsonResult GetItensMenu(int idMenu)
        {
            var itens = _context.ItensMenus
                .Where(i => i.IdMenu == idMenu)
                .Select(i => new { i.IdItensMenu, i.Descricao })
                .ToList();

            return Json(itens);
        }

        [HttpGet]
        public JsonResult VerificarPermissao(int idUsuario, int idMenu, int idItensMenu)
        {
            bool existe = _context.Permissoes.Any(p =>
                p.IdUsuario == idUsuario &&
                p.IdMenu == idMenu &&
                p.IdItensMenu == idItensMenu);

            return Json(new { existe });
        }
    }
}