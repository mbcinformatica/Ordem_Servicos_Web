using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Helpers;
using Ordem_Servicos_Web.Models;
using Ordem_Servicos_Web.Services;

namespace Ordem_Servicos_Web.Controllers.Cadastros
{
    public class UnidadesController(MeuDbContext context, ILogger<UnidadesController> logger, PermissaoService permissaoService) : Controller
    {
        private readonly MeuDbContext _context = context;
        private readonly ILogger<UnidadesController> _logger = logger;
        private readonly PermissaoService _permissaoService = permissaoService;

        // Index com paginação + pesquisa
        public IActionResult Index(int page = 1, string search = "", string column = "Descricao")
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);

            if (!_permissaoService.PodeExecutar(idUsuario, "CADASTROS", "UNIDADES DE MEDIDAS"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Acessar essa Tela.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index", "Home");
            }

            int pageSize = 10;

            var query = _context.Unidades.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = ApplySearchFilter(query, search, column);
            }

            query = ApplyOrdering(query, column);

            var unidades = query
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

            return View(unidades);
        }

        // Action para pesquisa Ajax
        public IActionResult Search(string search = "", string column = "Descricao")
        {
            var query = _context.Unidades.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = ApplySearchFilter(query, search, column);
            }

            query = ApplyOrdering(query, column);

            var unidades = query.ToList();

            return PartialView("_UnidadesTable", unidades);
        }

        // Método auxiliar para aplicar filtro
        private static IQueryable<Unidade> ApplySearchFilter(IQueryable<Unidade> query, string search, string column)
        {
            switch (column)
            {
                case "IdUnidade":
                    if (int.TryParse(search, out int id))
                        query = query.Where(un => un.IdUnidade == id);
                    break;
                default: // Descricao
                    query = query.Where(un => un.Descricao != null && un.Descricao.StartsWith(search));
                    break;
            }
            return query;
        }

        // Método auxiliar para aplicar ordenação
        private static IQueryable<Unidade> ApplyOrdering(IQueryable<Unidade> query, string column)
        {
            return column switch
            {
                "IdUnidade" => query.OrderBy(un => un.IdUnidade),
                _ => query.OrderBy(un => un.Descricao),
            };
        }

        // GET: Unidades/Create
        public IActionResult Create()
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeCriar(idUsuario, "CADASTROS", "UNIDADES DE MEDIDAS"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Executar essa Ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }
            return View();
        }

        // POST: Unidades/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Unidade unidade)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Verifica duplicidade de CPF/CNPJ
                    bool existe = _context.Unidades.Any(un => un.Descricao == unidade.Descricao);
                    if (existe)
                    {
                        TempData["Mensagem"] = $"A Unidade {unidade.Descricao} já Está Cadastrada.";
                        TempData["MensagemTipo"] = "aviso";
                        return View(unidade);
                    }

                    _context.Unidades.Add(unidade);
                    _context.SaveChanges();

                    TempData["Mensagem"] = "Unidade Incluída com Sucesso!";
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
                _logger.LogError(ex, "Erro ao Incluir Unidade no Banco de Dados.");
                TempData["Mensagem"] = "Ocorreu um Erro ao Salvar no Banco de Dados. Tente Novamente.";
            }
            TempData["MensagemTipo"] = "erro";

            return View(unidade);
        }

        // GET: Unidades/Alterar/5
        [HttpGet]
        public IActionResult Alterar(int id)
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeAlterar(idUsuario, "CADASTROS", "UNIDADES DE MEDIDAS"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Executar essa Ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }

            var unidade = _context.Unidades.FirstOrDefault(un => un.IdUnidade == id);
            if (unidade == null)
            {
                return NotFound();
            }
            return View(unidade);
        }

        // POST: Unidades/Alterar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Alterar(Unidade unidade)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Verifica duplicidade ao alterar (exceto o próprio registro)
                    bool existe = _context.Unidades.Any(un => un.Descricao == unidade.Descricao && un.IdUnidade != unidade.IdUnidade);
                    if (existe)
                    {
                        TempData["Mensagem"] = $"A Unidade {unidade.Descricao} já Está Cadastrada.";
                        TempData["MensagemTipo"] = "aviso";
                        return View(unidade);
                    }

                    _context.Unidades.Update(unidade); 
                    _context.SaveChanges();

                    TempData["Mensagem"] = "Unidade Alterada com Sucesso!";
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
                _logger.LogError(ex, "Erro ao Alterar Unidade no Banco de Dados.");
                TempData["Mensagem"] = "Ocorreu um Erro ao Salvar no Banco de Dados. Tente Novamente.";
            }
            TempData["MensagemTipo"] = "erro";
            return View(unidade);
        }

        // GET: Unidades/Excluir/5
        public IActionResult Excluir(int id)
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeExcluir(idUsuario, "CADASTROS", "UNIDADES DE MEDIDAS"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Executar essa Ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }

            var unidade = _context.Unidades.FirstOrDefault(un => un.IdUnidade == id);
            if (unidade == null)
            {
                return NotFound();
            }
            return View(unidade); // retorna view de confirmação
        }

        // POST: Unidades/Excluir/5
        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public IActionResult ExcluirConfirmado(int id)
        {
            try
            {
                var unidade = _context.Unidades.Find(id);
                if (unidade != null)
                {
                    _context.Unidades.Remove(unidade);
                    _context.SaveChanges();
                    TempData["Mensagem"] = "Unidade Excluida com Sucesso!";
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
        public async Task<JsonResult> VerificarDescricaoUnidade(string descricaoUnidade)
        {
            bool existe = _context.Unidades.Any(un => un.Descricao == descricaoUnidade);
            return Json(new { existe });
        }
    }
}