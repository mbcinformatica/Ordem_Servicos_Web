using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Helpers;
using Ordem_Servicos_Web.Models;
using Ordem_Servicos_Web.Services;

namespace Ordem_Servicos_Web.Controllers.Cadastros
{
    public class MarcasController(MeuDbContext context, ILogger<MarcasController> logger, PermissaoService permissaoService, LogService logService) : Controller
    {
        private readonly MeuDbContext _context = context;
        private readonly ILogger<MarcasController> _logger = logger;
        private readonly PermissaoService _permissaoService = permissaoService;
        private readonly LogService _logService = logService;

        // Index com paginação + pesquisa
        public IActionResult Index(int page = 1, string search = "", string column = "Descricao")
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);

            if (!_permissaoService.PodeExecutar(idUsuario, "CADASTROS", "MARCAS"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Acessar essa Tela.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index", "Home");
            }

            int pageSize = 10;

            var query = _context.Marcas.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = ApplySearchFilter(query, search, column);
            }

            query = ApplyOrdering(query, column);

            var marcas = query
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

            return View(marcas);
        }

        // Action para pesquisa Ajax
        public IActionResult Search(string search = "", string column = "Descricao")
        {
            var query = _context.Marcas.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = ApplySearchFilter(query, search, column);
            }

            query = ApplyOrdering(query, column);

            var marcas = query.ToList();

            return PartialView("_MarcasTable", marcas);
        }

        // Método auxiliar para aplicar filtro
        private static IQueryable<Marca> ApplySearchFilter(IQueryable<Marca> query, string search, string column)
        {
            switch (column)
            {
                case "IdMarca":
                    if (int.TryParse(search, out int id))
                        query = query.Where(ma => ma.IdMarca == id);
                    break;
                default: // Descricao
                    query = query.Where(ma => ma.Descricao.StartsWith(search));
                    break;
            }
            return query;
        }

        // Método auxiliar para aplicar ordenação
        private static IQueryable<Marca> ApplyOrdering(IQueryable<Marca> query, string column)
        {
            return column switch
            {
                "IdMarca" => query.OrderBy(ma => ma.IdMarca),
                _ => query.OrderBy(ma => ma.Descricao),
            };
        }

        // GET: Marcas/Create
        public IActionResult Create()
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeCriar(idUsuario, "CADASTROS", "MARCAS"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Executar essa Ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }
            return View();
        }

        // POST: Marcas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Marca marca)
        {
            try
            {
                ModelState.Remove("Modelos");

                if (ModelState.IsValid)
                {
                    // Verifica duplicidade de CPF/CNPJ
                    bool existe = _context.Marcas.Any(ma => ma.Descricao == marca.Descricao);
                    if (existe)
                    {
                        TempData["Mensagem"] = $"A Marca {marca.Descricao} já Está Cadastrada.";
                        TempData["MensagemTipo"] = "aviso";
                        return View(marca);
                    }

                    _context.Marcas.Add(marca);
                    _context.SaveChanges();

                    var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
                    _logService.Registrar(idUsuario, "Criar", "Marcas", marca.IdMarca, marca.Descricao, "Registro Criado");

                    TempData["Mensagem"] = "Marca Incluída com Sucesso!";
                    TempData["MensagemTipo"] = "sucesso";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Mensagem"] = "Ocorreu um Erro na Validação das Informações. Tente Novamente.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao Incluir Marca no Banco de Dados.");
                TempData["Mensagem"] = "Ocorreu um Erro ao Salvar no Banco de Dados. Tente Novamente.";
            }
            TempData["MensagemTipo"] = "erro";

            return View(marca);
        }

        // GET: Marcas/Alterar/5
        [HttpGet]
        public IActionResult Alterar(int id)
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeAlterar(idUsuario, "CADASTROS", "MARCAS"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Executar essa Ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }

            var marca = _context.Marcas.FirstOrDefault(ma => ma.IdMarca == id);
            if (marca == null)
            {
                return NotFound();
            }
            return View(marca);
        }

        // POST: Marcas/Alterar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Alterar(Marca marca)
        {
            try
            {
                ModelState.Remove("Modelos");

                if (ModelState.IsValid)

                {
                    // Verifica duplicidade ao alterar (exceto o próprio registro)
                    bool existe = _context.Marcas.Any(ma => ma.Descricao == marca.Descricao && ma.IdMarca != marca.IdMarca);
                    if (existe)
                    {
                        TempData["Mensagem"] = $"A Marca {marca.Descricao} já Está Cadastrada.";
                        TempData["MensagemTipo"] = "aviso";
                        return View(marca);
                    }

                    _context.Marcas.Update(marca); 
                    _context.SaveChanges();

                    var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
                    _logService.Registrar(idUsuario, "Alterar", "Marcas", marca.IdMarca, marca.Descricao, "Registro Alterado");

                    TempData["Mensagem"] = "Marca Alterada com Sucesso!";
                    TempData["MensagemTipo"] = "sucesso";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Mensagem"] = "Ocorreu um Erro na Validação das Informações. Tente Novamente.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao Alterar Marca no Banco de Dados.");
                TempData["Mensagem"] = "Ocorreu um Erro ao Salvar no Banco de Dados. Tente Novamente.";
            }
            TempData["MensagemTipo"] = "erro";
            return View(marca);
        }

        // GET: Marcas/Excluir/5
        public IActionResult Excluir(int id)
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeExcluir(idUsuario, "CADASTROS", "MARCAS"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Executar essa Ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }

            var marca = _context.Marcas.FirstOrDefault(ma => ma.IdMarca == id);
            if (marca == null)
            {
                return NotFound();
            }
            return View(marca); // retorna view de confirmação
        }

        // POST: Marcas/Excluir/5
        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public IActionResult ExcluirConfirmado(int id)
        {
            try
            {
                var marca = _context.Marcas.Find(id);
                if (marca != null)
                {

                    _context.Marcas.Remove(marca);
                    _context.SaveChanges();

                    var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
                    _logService.Registrar(idUsuario, "Excluir", "Marcas", marca.IdMarca, marca.Descricao, "Registro Excluido");

                    TempData["Mensagem"] = "Marca Excluida com Sucesso!";
                    TempData["MensagemTipo"] = "sucesso";
                }
            }
            catch
            {
                TempData["Mensagem"] = "Ocorreu um Erro ao Excluir no Banco de Dados. Tente Novamente.";
                TempData["MensagemTipo"] = "erro";
            }
            return RedirectToAction("Index");
        }

         // GET: Verificar Existência de descricão
        [HttpGet]
        public async Task<JsonResult> VerificarDescricaoMarca(string descricaoMarca)
        {
            bool existe = _context.Marcas.Any(ma => ma.Descricao == descricaoMarca);
            return Json(new { existe });
        }

    }
}