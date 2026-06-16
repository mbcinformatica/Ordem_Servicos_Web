using Microsoft.AspNetCore.Mvc;
using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Helpers;
using Ordem_Servicos_Web.Models;
using Ordem_Servicos_Web.Services;

namespace Ordem_Servicos_Web.Controllers.Cadastros
{
    public class ClientesController(MeuDbContext context, ILogger<ClientesController> logger, PermissaoService permissaoService, EntidadesService entidadesService, LogService logService) : Controller
    {
        private readonly MeuDbContext _context = context;
        private readonly ILogger<ClientesController> _logger = logger;
        private readonly PermissaoService _permissaoService = permissaoService;
        private readonly EntidadesService _entidadesService = entidadesService;
        private readonly LogService _logService = logService;

        // Index com paginação + pesquisa
        public IActionResult Index(int page = 1, string search = "", string column = "NomeRazaoSocial")
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);

            if (!_permissaoService.PodeExecutar(idUsuario, "CADASTROS", "CLIENTES"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Acessar essa Tela.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index", "Home");
            }


            int pageSize = 10;

            var query = _context.Clientes.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = ApplySearchFilter(query, search, column);
            }

            query = ApplyOrdering(query, column);

            var clientes = query
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

            return View(clientes);
        }

        // Action para pesquisa Ajax
        public IActionResult Search(string search = "", string column = "NomeRazaoSocial")
        {
            var query = _context.Clientes.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = ApplySearchFilter(query, search, column);
            }

            query = ApplyOrdering(query, column);

            var clientes = query.ToList();

            return PartialView("_ClientesTable", clientes);
        }

        // Método auxiliar para aplicar filtro
        private static IQueryable<Cliente> ApplySearchFilter(IQueryable<Cliente> query, String search, string column)
        {
            search = search.ToLower().Trim();

            switch (column)
            {
                case "IdCliente":
                    if (int.TryParse(search, out int id))
                        query = query.Where(c => c.IdCliente == id);
                    break;
                case "CpfCnpj":
                    query = query.Where(c => c.CpfCnpj.StartsWith(search, StringComparison.CurrentCultureIgnoreCase));
                    break;
                default: // NomeRazaoSocial
                    query = query.Where(c => c.NomeRazaoSocial.StartsWith(search, StringComparison.CurrentCultureIgnoreCase));
                    break;
            }
            return query;
        }

        // Método auxiliar para aplicar ordenação
        private static IQueryable<Cliente> ApplyOrdering(IQueryable<Cliente> query, string column) => column switch
        {
            "IdCliente" => query.OrderBy(c => c.IdCliente),
            "CpfCnpj" => query.OrderBy(c => c.CpfCnpj),
            _ => query.OrderBy(c => c.NomeRazaoSocial),
        };

        // GET: Clientes/Create
        public IActionResult Create()
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeCriar(idUsuario, "CADASTROS", "CLIENTES"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Executar essa Ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }
            return View();
        }

        // POST: Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Cliente cliente)
        {

            try
            {
                var normalizarCampos = new[] { "CpfCnpj", "Cep", "FoneFixo", "FoneCelular", "Email" };
                _entidadesService.NormalizarCampos(cliente, normalizarCampos);

                if (ModelState.IsValid)
                {
                    cliente.DataCadastro = DateTime.Now;
                    _context.Clientes.Add(cliente);
                    _context.SaveChanges();

                    var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
                    _logService.Registrar(idUsuario, "Criar", "Clientes", cliente.IdCliente, cliente.NomeRazaoSocial, "Registro Criado");

                    TempData["Mensagem"] = "Cliente Incluído com Sucesso!";
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
                _logger.LogError(ex, "Erro ao Incluir Cliente no Banco de Dados.");
                TempData["Mensagem"] = "Ocorreu um Erro ao Salvar no Banco de Dados. Tente Novamente.";
            }
            TempData["MensagemTipo"] = "erro";

            return View(cliente);
        }

        // GET: Clientes/Alterar/5
        [HttpGet]
        public IActionResult Alterar(int id)
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeAlterar(idUsuario, "CADASTROS", "CLIENTES"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Executar essa Ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }

            var cliente = _context.Clientes.Find(id);
            if (cliente == null) return NotFound();

            return View(cliente);
        }

        // POST: Clientes/Alterar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Alterar(Cliente cliente)
        {
            try
            {
                var normalizarCampos = new[] { "CpfCnpj", "Cep", "FoneFixo", "FoneCelular", "Email" };
                _entidadesService.NormalizarCampos(cliente, normalizarCampos);
                if (ModelState.IsValid)
                {
                    _context.Clientes.Update(cliente); 
                    _context.SaveChanges();

                    var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
                    _logService.Registrar(idUsuario, "Alterar", "Clientes", cliente.IdCliente, cliente.NomeRazaoSocial, "Registro Alterado");

                    TempData["Mensagem"] = "Cliente Alterado com Sucesso!";
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
                _logger.LogError(ex, "Erro ao Alterar Cliente no Banco de Dados.");
                TempData["Mensagem"] = "Ocorreu um Erro ao Salvar no Banco de Dados. Tente Novamente.";
            }
            TempData["MensagemTipo"] = "erro";
            return View(cliente);
        }

        // GET: Clientes/Excluir/5
        public IActionResult Excluir(int id)
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeExcluir(idUsuario, "CADASTROS", "CLIENTES"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Executar essa Ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }

            var cliente = _context.Clientes.FirstOrDefault(c => c.IdCliente == id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente); // retorna view de confirmação
        }

        // POST: Clientes/Excluir/5
        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public IActionResult ExcluirConfirmado(int id)
        {
            try
            {
                var cliente = _context.Clientes.Find(id);
                if (cliente != null)
                {
                    _context.Clientes.Remove(cliente);
                    _context.SaveChanges();

                    var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
                    _logService.Registrar(idUsuario, "Excluir", "Clientes", cliente.IdCliente, cliente.NomeRazaoSocial, "Registro Excluido");

                    TempData["Mensagem"] = "Cliente Excluido com Sucesso!";
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