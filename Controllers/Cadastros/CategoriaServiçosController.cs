using Microsoft.AspNetCore.Mvc;
using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Helpers;
using Ordem_Servicos_Web.Models;
using Ordem_Servicos_Web.Services;
using System.Text.RegularExpressions;

namespace Ordem_Servicos_Web.Controllers.Cadastros
{

    public class CategoriaServicosController(MeuDbContext context, ILogger<CategoriaServicosController> logger, PermissaoService permissaoService, LogService logService) : Controller
    {
        private readonly MeuDbContext _context = context;
        private readonly ILogger<CategoriaServicosController> _logger = logger;
        private readonly PermissaoService _permissaoService = permissaoService;
        private readonly LogService _logService = logService;

        // Index com paginação + pesquisa
        public IActionResult Index(int page = 1, string search = "", string column = "Descricao")
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);

            if (!_permissaoService.PodeExecutar(idUsuario, "CADASTROS", "CATEGORIA DE SERVIÇOS"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Acessar essa Tela.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index", "Home");
            }

            int pageSize = 10;

            var query = _context.CategoriaServicos.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = ApplySearchFilter(query, search, column);
            }

            query = ApplyOrdering(query, column);

            var categoriaServicos = query
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

            return View(categoriaServicos);
        }

        // Action para pesquisa Ajax
        public IActionResult Search(string search = "", string column = "Descricao")
        {
            var query = _context.CategoriaServicos.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = ApplySearchFilter(query, search, column);
            }

            query = ApplyOrdering(query, column);

            var categoriaServicos = query.ToList();

            return PartialView("_CategoriaServicosTable", categoriaServicos);
        }

        // Método auxiliar para aplicar filtro
        private static IQueryable<CategoriaServico> ApplySearchFilter(IQueryable<CategoriaServico> query, string search, string column)
        {
            switch (column)
            {
                case "IdCategoriaServico":
                    if (int.TryParse(search, out int id))
                        query = query.Where(cs => cs.IdCategoriaServico == id);
                    break;
                default: // Descricao
                    query = query.Where(cs => cs.Descricao.StartsWith(search));
                    break;
            }
            return query;
        }

        // Método auxiliar para aplicar ordenação
        private static IQueryable<CategoriaServico> ApplyOrdering(IQueryable<CategoriaServico> query, string column)
        {
            return column switch
            {
                "IdCategoriaServico" => query.OrderBy(cs => cs.IdCategoriaServico),
                _ => query.OrderBy(cs => cs.Descricao),
            };
        }

        // GET: CategoriaServicos/Create
        public IActionResult Create()
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeCriar(idUsuario, "CADASTROS", "CATEGORIA DE SERVIÇOS"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Executar essa Ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }
            return View();
        }

        // POST: CategoriaServicos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CategoriaServico categoriaServico)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Verifica duplicidade de CPF/CNPJ
                    bool existe = _context.CategoriaServicos.Any(cs => cs.Descricao == categoriaServico.Descricao);
                    if (existe)
                    {
                        TempData["Mensagem"] = $"A Categoria de Serviços {categoriaServico.Descricao} já Está Cadastrada.";
                        TempData["MensagemTipo"] = "aviso";
                        return View(categoriaServico);
                    }

                    _context.CategoriaServicos.Add(categoriaServico);
                    _context.SaveChanges();

                    var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
                    _logService.Registrar(idUsuario, "Criar", "Categoria de Serviços", categoriaServico.IdCategoriaServico,categoriaServico.Descricao, "Registro Criado");

                    TempData["Mensagem"] = "Categoria de Serviços Incluída com Sucesso!";
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
                _logger.LogError(ex, "Erro ao Incluir Categoria de Serviços no Banco de Dados.");
                TempData["Mensagem"] = "Ocorreu um Erro ao Salvar no Banco de Dados. Tente Novamente.";
            }
            TempData["MensagemTipo"] = "erro";

            return View(categoriaServico);
        }

        // GET: CategoriaServicos/Alterar/5
        [HttpGet]
        public IActionResult Alterar(int id)
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeAlterar(idUsuario, "CADASTROS", "CATEGORIA DE SERVIÇOS"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Executar essa Ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }

            var categoriaServico = _context.CategoriaServicos.FirstOrDefault(cs => cs.IdCategoriaServico == id);
            if (categoriaServico == null)
            {
                return NotFound();
            }
            return View(categoriaServico);
        }

        // POST: CategoriaServicos/Alterar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Alterar(CategoriaServico categoriaServico)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Verifica duplicidade ao alterar (exceto o próprio registro)
                    bool existe = _context.CategoriaServicos.Any(cs => cs.Descricao == categoriaServico.Descricao && cs.IdCategoriaServico != categoriaServico.IdCategoriaServico);
                    if (existe)
                    {
                        TempData["Mensagem"] = $"A Categoria de Serviços {categoriaServico.Descricao} já Está Cadastrada.";
                        TempData["MensagemTipo"] = "aviso";
                        return View(categoriaServico);
                    }

                    _context.CategoriaServicos.Update(categoriaServico); 
                    _context.SaveChanges();

                    var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
                    _logService.Registrar(idUsuario, "Alterar", "Categoria de Serviços", categoriaServico.IdCategoriaServico, categoriaServico.Descricao, "Registro Alterado");

                    TempData["Mensagem"] = "Categoria de Serviços Alterada com Sucesso!";
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
                _logger.LogError(ex, "Erro ao Alterar Categoria de Serviços no Banco de Dados.");
                TempData["Mensagem"] = "Ocorreu um Erro ao Salvar no Banco de Dados. Tente Novamente.";
            }
            TempData["MensagemTipo"] = "erro";
            return View(categoriaServico);
        }

        // GET: CategoriaServicos/Excluir/5
        public IActionResult Excluir(int id)
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeExcluir(idUsuario, "CADASTROS", "CATEGORIA DE SERVIÇOS"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Executar essa Ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }

            var categoriaServico = _context.CategoriaServicos.FirstOrDefault(cs => cs.IdCategoriaServico == id);
            if (categoriaServico == null)
            {
                return NotFound();
            }
            return View(categoriaServico); // retorna view de confirmação
        }

        // POST: CategoriaServicos/Excluir/5
        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public IActionResult ExcluirConfirmado(int id)
        {
            try
            {
                var categoriaServico = _context.CategoriaServicos.Find(id);
                if (categoriaServico != null)
                {
                    _context.CategoriaServicos.Remove(categoriaServico);
                    _context.SaveChanges();

                    var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
                    _logService.Registrar(idUsuario, "Excluir", "Categoria de Serviços", categoriaServico.IdCategoriaServico, categoriaServico.Descricao, "Registro Excluido");

                    TempData["Mensagem"] = "Categoria de Serviços Excluida com Sucesso!";
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
        public async Task<JsonResult> VerificarDescricaoCategoriaServico(string descricaoCategoriaServico)
        {
            bool existe = _context.CategoriaServicos.Any(cs => cs.Descricao == descricaoCategoriaServico);
            return Json(new { existe });
        }
    }
}