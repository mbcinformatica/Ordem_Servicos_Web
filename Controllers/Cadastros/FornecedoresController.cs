using Microsoft.AspNetCore.Mvc;
using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Helpers;
using Ordem_Servicos_Web.Models;
using Ordem_Servicos_Web.Services;

namespace Ordem_Servicos_Web.Controllers.Cadastros
{
    public class FornecedoresController(MeuDbContext context, ILogger<FornecedoresController> logger, PermissaoService permissaoService, EntidadesService entidadesService, LogService logService) : Controller
    {
        private readonly MeuDbContext _context = context;
        private readonly ILogger<FornecedoresController> _logger = logger;
        private readonly PermissaoService _permissaoService = permissaoService;
        private readonly EntidadesService _entidadesService = entidadesService;
        private readonly LogService _logService = logService;

        // Index com paginação + pesquisa
        public IActionResult Index(int page = 1, string search = "", string column = "NomeRazaoSocial")
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);

            if (!_permissaoService.PodeExecutar(idUsuario, "CADASTROS", "FORNECEDORES"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Acessar essa Tela.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index", "Home");
            }

            int pageSize = 10;

            var query = _context.Fornecedores.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = ApplySearchFilter(query, search, column);
            }

            query = ApplyOrdering(query, column);

            var fornecedores = query
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

            return View(fornecedores);
        }

        // Action para pesquisa Ajax
        public IActionResult Search(string search = "", string column = "NomeRazaoSocial")
        {
            var query = _context.Fornecedores.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = ApplySearchFilter(query, search, column);
            }

            query = ApplyOrdering(query, column);

            var fornecedores = query.ToList();

            return PartialView("_FornecedoresTable", fornecedores);
        }

        // Método auxiliar para aplicar filtro
        private static IQueryable<Fornecedor> ApplySearchFilter(IQueryable<Fornecedor> query, string search, string column)
        {
            switch (column)
            {
                case "IdFornecedor":
                    if (int.TryParse(search, out int id))
                        query = query.Where(f => f.IdFornecedor == id);
                    break;
                case "CpfCnpj":
                    query = query.Where(f => f.CpfCnpj.StartsWith(search));
                    break;
                default: // NomeRazaoSocial
                    query = query.Where(f => f.NomeRazaoSocial.StartsWith(search));
                    break;
            }
            return query;
        }

        // Método auxiliar para aplicar ordenação
        private static IQueryable<Fornecedor> ApplyOrdering(IQueryable<Fornecedor> query, string column)
        {
            return column switch
            {
                "IdFornecedor" => query.OrderBy(f => f.IdFornecedor),
                "CpfCnpj" => query.OrderBy(f => f.CpfCnpj),
                _ => query.OrderBy(f => f.NomeRazaoSocial),
            };
        }

        // GET: Fornecedores/Create
        public IActionResult Create()
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeCriar(idUsuario, "CADASTROS", "FORNECEDORES"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Executar essa Ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }
            return View();
        }

        // POST: Fornecedores/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Fornecedor fornecedor)
        {
            try
            {
                _entidadesService.NormalizarCampos(fornecedor);

                if (ModelState.IsValid)
                {
                    fornecedor.DataCadastro = DateTime.Now;
                    _context.Fornecedores.Add(fornecedor);
                    _context.SaveChanges();

                    var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
                    _logService.Registrar(idUsuario, "Criar", "Fornecedores", fornecedor.IdFornecedor, fornecedor.NomeRazaoSocial, "Registro Criado");

                    TempData["Mensagem"] = "Fornecedor Incluído com Sucesso!";
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
                _logger.LogError(ex, "Erro ao Incluir Fornecedor no Banco de Dados.");
                TempData["Mensagem"] = "Ocorreu um Erro ao Salvar no Banco de Dados. Tente Novamente.";
            }
            TempData["MensagemTipo"] = "erro";

            return View(fornecedor);
        }

        // GET: Fornecedores/Alterar/5
        [HttpGet]
        public IActionResult Alterar(int id)
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeAlterar(idUsuario, "CADASTROS", "FORNECEDORES"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Executar essa Ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }

            var fornecedor = _context.Fornecedores.FirstOrDefault(f => f.IdFornecedor == id);
            if (fornecedor == null)
            {
                return NotFound();
            }
            return View(fornecedor);
        }

        // POST: Fornecedores/Alterar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Alterar(Fornecedor fornecedor)
        {
            try
            {
                _entidadesService.NormalizarCampos(fornecedor);

                if (ModelState.IsValid)
                {
                    _context.Fornecedores.Update(fornecedor); 
                    _context.SaveChanges();

                    var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
                    _logService.Registrar(idUsuario, "Alterar", "Fornecedores", fornecedor.IdFornecedor, fornecedor.NomeRazaoSocial, "Registro Alterado");

                    TempData["Mensagem"] = "Fornecedor Alterado com Sucesso!";
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
                _logger.LogError(ex, "Erro ao Alterar Fornecedor no Banco de Dados.");
                TempData["Mensagem"] = "Ocorreu um Erro ao Salvar no Banco de Dados. Tente Novamente.";
            }
            TempData["MensagemTipo"] = "erro";
            return View(fornecedor);
        }

        // GET: Fornecedores/Excluir/5
        public IActionResult Excluir(int id)
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeExcluir(idUsuario, "CADASTROS", "FORNECEDORES"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Executar essa Ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }

            var fornecedor = _context.Fornecedores.FirstOrDefault(f => f.IdFornecedor == id);
            if (fornecedor == null)
            {
                return NotFound();
            }
            return View(fornecedor); // retorna view de confirmação
        }

        // POST: Fornecedores/Excluir/5
        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public IActionResult ExcluirConfirmado(int id)
        {
            try
            {
                var fornecedor = _context.Fornecedores.Find(id);
                if (fornecedor != null)
                {
                    _context.Fornecedores.Remove(fornecedor);
                    _context.SaveChanges();

                    var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
                    _logService.Registrar(idUsuario, "Excluir", "Fornecedores", fornecedor.IdFornecedor, fornecedor.NomeRazaoSocial, "Registro Excluido");

                    TempData["Mensagem"] = "Fornecedor Excluido com Sucesso!";
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
    }
}