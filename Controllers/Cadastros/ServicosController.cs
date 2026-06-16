using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Helpers;
using Ordem_Servicos_Web.Models;
using Ordem_Servicos_Web.Services;

namespace Ordem_Servicos_Web.Controllers.Cadastros
{
    public class ServicosController(MeuDbContext context, ILogger<ServicosController> logger, PermissaoService permissaoService) : Controller
    {
        private readonly MeuDbContext _context = context;
        private readonly ILogger<ServicosController> _logger = logger;
        private readonly PermissaoService _permissaoService = permissaoService;

        // Index: Listagem com Paginação e Pesquisa
        public IActionResult Index(int page = 1, string search = "", string column = "Descricao")
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);

            if (!_permissaoService.PodeExecutar(idUsuario, "CADASTROS", "SERVIÇOS"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Acessar essa Tela.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index", "Home");
            }


            int pageSize = 10;

            var query = _context.Servicos
                .Include(se => se.CategoriaServico)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = ApplySearchFilter(query, search, column);
            }

            query = ApplyOrdering(query, column);

            var servicos = query
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

            return View(servicos);
        }

        // Action para pesquisa Ajax
        public IActionResult Search(string search = "", string column = "Descricao")
        {
            var query = _context.Servicos
                .Include(se => se.CategoriaServico)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = ApplySearchFilter(query, search, column);
            }

            query = ApplyOrdering(query, column);

            var servicos = query.ToList();

            return PartialView("_ServicosTable", servicos);
        }

        // Método auxiliar para aplicar filtro
        private static IQueryable<Servico> ApplySearchFilter(IQueryable<Servico> query, string search, string column)
        {
            switch (column)
            {

                case "IdServico":
                    if (int.TryParse(search, out int id))
                        query = query.Where(se => se.IdServico == id);
                    break;
                case "CategoriaServicoDescricao":
                    query = query.Where(se => se.CategoriaServico != null && se.CategoriaServico.Descricao.StartsWith(search));
                    break;
                default: // Descricao
                    query = query.Where(se => se.Descricao.StartsWith(search));
                    break;
            }
            return query;
        }

        // Método auxiliar para aplicar ordenação
        private static IQueryable<Servico> ApplyOrdering(IQueryable<Servico> query, string column)
        {
            return column switch
            {
                "IdServico" => query.OrderBy(se => se.IdServico),
                "IdCategoriaServico" => query.OrderBy(se => se.IdCategoriaServico),
                _ => query.OrderBy(se => se.Descricao),
            };
        }

        // GET: Servicos/Create
        public IActionResult Create()
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeCriar(idUsuario, "CADASTROS", "SERVIÇOS"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Executar essa Ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }
            return View();
        }

        // POST: Servicos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Servico servico)
        {
            try
            {
                ModelState.Remove("CategoriaServico");

                if (ModelState.IsValid)
                {
                    // Verifica duplicidade de CPF/CNPJ
                    bool existe = _context.Servicos.Any(se =>
                        se.IdCategoriaServico == servico.IdCategoriaServico &&
                        se.Descricao == servico.Descricao);
                    if (existe)
                    {
                        TempData["Mensagem"] = $"Já existe um Servico '{servico.Descricao}' para esta Marca.";
                        TempData["MensagemTipo"] = "aviso";
                        return View(servico);
                    }

                    _context.Servicos.Add(servico);
                    _context.SaveChanges();

                    TempData["Mensagem"] = "Serviço Incluída com Sucesso!";
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
                _logger.LogError(ex, "Erro ao Incluir Servico no Banco de Dados.");
                TempData["Mensagem"] = "Ocorreu um Erro ao Salvar no Banco de Dados. Tente Novamente.";
            }
            TempData["MensagemTipo"] = "erro";

            return View(servico);
        }

        // GET: Servicos/Alterar/5
        [HttpGet]
        public IActionResult Alterar(int id)
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeAlterar(idUsuario, "CADASTROS", "SERVIÇOS"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Executar essa Ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }

            var servico = _context.Servicos
                .Include(se => se.CategoriaServico)
                .FirstOrDefault(se => se.IdServico == id);

            if (servico == null)
            {
                return NotFound();
            }
            return View(servico);
        }

        // POST: Servicos/Alterar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Alterar(Servico servico)
        {
            try
            {
                ModelState.Remove("CategoriaServico");

                if (ModelState.IsValid)
                {
                    // Verifica duplicidade apenas na mesma Marca
                    bool existe = _context.Servicos.Any(se =>
                        se.IdCategoriaServico == servico.IdCategoriaServico &&
                        se.Descricao == servico.Descricao &&
                        se.IdServico != servico.IdServico);
                    
                    if (existe)
                    {
                        TempData["Mensagem"] = $"Já existe um Servico '{servico.Descricao}' para esta Marca.";
                        TempData["MensagemTipo"] = "aviso";
                        return View(servico);
                    }

                    _context.Servicos.Update(servico); 
                    _context.SaveChanges();

                    TempData["Mensagem"] = "Servico Alterado com Sucesso!";
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
                _logger.LogError(ex, "Erro ao Alterar Servico no Banco de Dados.");
                TempData["Mensagem"] = "Ocorreu um Erro ao Salvar no Banco de Dados. Tente Novamente.";
            }
            TempData["MensagemTipo"] = "erro";
            return View(servico);
        }

        // GET: Servicos/Excluir/5
        public IActionResult Excluir(int id)
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeExcluir(idUsuario, "CADASTROS", "SERVIÇOS"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Executar essa Ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }

            var servico = _context.Servicos
                .Include(se => se.CategoriaServico)
                .FirstOrDefault(se => se.IdServico == id);

            if (servico == null)
                return NotFound();

            return View(servico);
        }

        // POST: Servicos/Excluir/5
        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public IActionResult ExcluirConfirmado(int id)
        {
            try
            {
                var servico = _context.Servicos.Find(id);
                if (servico != null)
                {
                    _context.Servicos.Remove(servico);
                    _context.SaveChanges();
                    TempData["Mensagem"] = "Serviço excluído com sucesso!";
                    TempData["MensagemTipo"] = "sucesso";
                }
            }
            catch
            {
                TempData["Mensagem"] = "Ocorreu um erro ao excluir no banco de dados. Tente novamente.";
                TempData["MensagemTipo"] = "erro";
            }
            return RedirectToAction("Index");
        }

        // GET: Servicos/GetCategoriaServico
        [HttpGet]
        public JsonResult GetCategoriaServico()
        {
            var categoriaServicos = _context.CategoriaServicos
                .Select(cs => new { idCategoriaServico = cs.IdCategoriaServico, descricao = cs.Descricao })
                .ToList();

            return Json(categoriaServicos);
        }

        // GET: Verificar Existência de descricão
        [HttpGet]
        public JsonResult VerificarDescricaoServico(int idCategoriaServico, string descricaoServico)
        {
            bool existe = _context.Servicos.Any(se => se.IdCategoriaServico == idCategoriaServico &&
                se.Descricao == descricaoServico);

            return Json(new { existe });
        }


        // GET: Verificar Existência de Codigo Base
        [HttpGet]
        public async Task<JsonResult> VerificarCodigoBase(string codigoBase)
        {
            bool existe = _context.Servicos.Any(se => se.IdCodigoBase == codigoBase);
            return Json(new { existe });
        }
    }
}