using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Helpers;
using Ordem_Servicos_Web.Models;
using Ordem_Servicos_Web.Services;
using System.Text.RegularExpressions;

namespace Ordem_Servicos_Web.Controllers.Cadastros
{
    public class ModelosController(MeuDbContext context, ILogger<ModelosController> logger, PermissaoService permissaoService, LogService logService) : Controller
    {
        private readonly MeuDbContext _context = context;
        private readonly ILogger<ModelosController> _logger = logger;
        private readonly PermissaoService _permissaoService = permissaoService;
        private readonly LogService _logService = logService;

        // Index: Listagem com Paginação e Pesquisa
        public IActionResult Index(int page = 1, string search = "", string column = "Descricao")
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);

            if (!_permissaoService.PodeExecutar(idUsuario, "CADASTROS", "MODELOS"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Acessar essa Tela.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index", "Home");
            }

            int pageSize = 10;

            var query = _context.Modelos
                .Include(mo => mo.Marca)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = ApplySearchFilter(query, search, column);
            }

            query = ApplyOrdering(query, column);

            var modelos = query
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

            return View(modelos);
        }


        // Action para pesquisa Ajax
        public IActionResult Search(string search = "", string column = "Descricao")
        {
            var query = _context.Modelos
                .Include(mo => mo.Marca)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = ApplySearchFilter(query, search, column);
            }

            query = ApplyOrdering(query, column);

            var modelos = query.ToList();

            return PartialView("_ModelosTable", modelos);
        }

        // Método auxiliar para aplicar filtro
        private static IQueryable<Modelo> ApplySearchFilter(IQueryable<Modelo> query, string search, string column)
        {
            switch (column)
            {
                case "IdModelo":
                    if (int.TryParse(search, out int id))
                        query = query.Where(mo => mo.IdModelo == id);
                    break;
                case "MarcaDescricao":
                    query = query.Where(mo => mo.Marca != null && mo.Marca.Descricao.StartsWith(search));
                    break;
                default: // Descricao
                    query = query.Where(mo => mo.Descricao.StartsWith(search));
                    break;
            }
            return query;
        }

        // Método auxiliar para aplicar ordenação
        private static IQueryable<Modelo> ApplyOrdering(IQueryable<Modelo> query, string column)
        {
            return column switch
            {
                "IdModelo" => query.OrderBy(mo => mo.IdModelo),
                "IdMarca" => query.OrderBy(mo => mo.IdMarca),
                _ => query.OrderBy(mo => mo.Descricao),
            };
        }

        // GET: Modelos/Create
        public IActionResult Create()
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeCriar(idUsuario, "CADASTROS", "MODELOS"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Executar essa Ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }
            return View();
        }

        // POST: Modelos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Modelo modelo)
        {
            try
            {
                ModelState.Remove("Marca");

                if (ModelState.IsValid)
                {
                    // Verifica duplicidade de CPF/CNPJ
                    bool existe = _context.Modelos.Any(mo =>
                        mo.IdMarca == modelo.IdMarca &&
                        mo.Descricao == modelo.Descricao);
                    if (existe)
                    {
                        TempData["Mensagem"] = $"Já existe um Modelo '{modelo.Descricao}' para esta Marca.";
                        TempData["MensagemTipo"] = "aviso";
                        return View(modelo);
                    }

                    _context.Modelos.Add(modelo);
                    _context.SaveChanges();

                    var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
                    _logService.Registrar(idUsuario, "Criar", "Modelos", modelo.IdModelo, modelo.Descricao, "Registro Criado");

                    TempData["Mensagem"] = "Modelo Incluída com Sucesso!";
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
                _logger.LogError(ex, "Erro ao Incluir Modelo no Banco de Dados.");
                TempData["Mensagem"] = "Ocorreu um Erro ao Salvar no Banco de Dados. Tente Novamente.";
            }
            TempData["MensagemTipo"] = "erro";

            return View(modelo);
        }

        // GET: Modelos/Alterar/5
        [HttpGet]
        public IActionResult Alterar(int id)
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeAlterar(idUsuario, "CADASTROS", "MODELOS"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Executar essa Ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }

            var modelo = _context.Modelos
                .Include(mo => mo.Marca)
                .FirstOrDefault(mo => mo.IdModelo == id);

            if (modelo == null)
            {
                return NotFound();
            }
            return View(modelo);
        }

        // POST: Modelos/Alterar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Alterar(Modelo modelo)
        {
            try
            {
                ModelState.Remove("Marca");

                if (ModelState.IsValid)
                {
                    // Verifica duplicidade apenas na mesma Marca
                    bool existe = _context.Modelos.Any(mo =>
                        mo.IdMarca == modelo.IdMarca &&
                        mo.Descricao == modelo.Descricao &&
                        mo.IdModelo != modelo.IdModelo);
                    
                    if (existe)
                    {
                        TempData["Mensagem"] = $"Já existe um Modelo '{modelo.Descricao}' para esta Marca.";
                        TempData["MensagemTipo"] = "aviso";
                        return View(modelo);
                    }

                    _context.Modelos.Update(modelo); 
                    _context.SaveChanges();

                    var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
                    _logService.Registrar(idUsuario, "Alterar", "Modelos", modelo.IdModelo, modelo.Descricao, "Registro Alterado");

                    TempData["Mensagem"] = "Modelo Alterado com Sucesso!";
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
                _logger.LogError(ex, "Erro ao Alterar Modelo no Banco de Dados.");
                TempData["Mensagem"] = "Ocorreu um Erro ao Salvar no Banco de Dados. Tente Novamente.";
            }
            TempData["MensagemTipo"] = "erro";
            return View(modelo);
        }

        // GET: Modelos/Excluir/5
        public IActionResult Excluir(int id)
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeExcluir(idUsuario, "CADASTROS", "MODELOS"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Executar essa Ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }

            var modelo = _context.Modelos
                .Include(mo => mo.Marca)
                .FirstOrDefault(mo => mo.IdModelo == id);

            if (modelo == null)
            {
                return NotFound();
            }
            return View(modelo); // retorna view de confirmação
        }

        // POST: Modelos/Excluir/5
        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public IActionResult ExcluirConfirmado(int id)
        {
            try
            {
                var modelo = _context.Modelos.Find(id);
                if (modelo != null)
                {
                    _context.Modelos.Remove(modelo);
                    _context.SaveChanges();

                    var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
                    _logService.Registrar(idUsuario, "Excluir", "Modelos", modelo.IdModelo, modelo.Descricao, "Registro Excluido");

                    TempData["Mensagem"] = "Modelo Excluido com Sucesso!";
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

        [HttpGet]
        public JsonResult GetMarcas()
        {
            var marcas = _context.Marcas
                .Select(ma => new { idMarca = ma.IdMarca, descricao = ma.Descricao })
                .ToList();

            return Json(marcas);
        }

        // GET: Verificar Existência de descricão
        [HttpGet]
        public JsonResult VerificarDescricaoModelo(int idMarca, string descricaoModelo)
        {
            bool existe = _context.Modelos.Any(mo =>
                mo.IdMarca == idMarca &&
                mo.Descricao == descricaoModelo);

            return Json(new { existe });
        }

    }
}