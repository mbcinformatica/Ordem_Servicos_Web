using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
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


=======
using MySqlX.XDevAPI;
using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Models;
using System.Text.RegularExpressions;

namespace Ordem_Servicos_Web.Controllers.Cadastros
{
    public class ClientesController : Controller
    {
        private readonly MeuDbContext _context;
        private readonly ILogger<ClientesController> _logger;

        public ClientesController(MeuDbContext context, ILogger<ClientesController> logger)
        {
            _context = context;
            _logger = logger;
        }
        // Index com paginação + pesquisa
        public IActionResult Index(int page = 1, string search = "")
        {
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
            int pageSize = 10;

            var query = _context.Clientes.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
<<<<<<< HEAD
                query = ApplySearchFilter(query, search, column);
            }

            query = ApplyOrdering(query, column);
=======
                query = query.Where(c => c.NomeRazaoSocial.StartsWith(search));
            }

            query = query.OrderBy(c => c.NomeRazaoSocial);
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7

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
<<<<<<< HEAD
            ViewBag.Column = column;

            return View(clientes);
        }

        // Action para pesquisa Ajax
        public IActionResult Search(string search = "", string column = "NomeRazaoSocial")
=======

            return View(clientes);
        }
        // Action para pesquisa Ajax
        public IActionResult Search(string search = "")
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
        {
            var query = _context.Clientes.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
<<<<<<< HEAD
                query = ApplySearchFilter(query, search, column);
            }

            query = ApplyOrdering(query, column);
=======
                query = query.Where(c => c.NomeRazaoSocial.StartsWith(search));
            }

            query = query.OrderBy(c => c.NomeRazaoSocial);
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7

            var clientes = query.ToList();

            return PartialView("_ClientesTable", clientes);
        }
<<<<<<< HEAD

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

=======
        // GET: Clientes/Create
        public IActionResult Create()
        {
            return View();
        }
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
        // POST: Clientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Cliente cliente)
        {
<<<<<<< HEAD

            try
            {
                _entidadesService.NormalizarCampos(cliente);

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

=======
            NormalizarDados(cliente);

            if (ModelState.IsValid)
            {
                try
                {
                    // Verifica duplicidade de CPF/CNPJ
                    bool existe = _context.Clientes.Any(c => c.CpfCnpj == cliente.CpfCnpj);
                if (existe)
                {
                    ModelState.AddModelError("CpfCnpj", "Este CPF/CNPJ já está Cadastrado.");
                    return View(cliente);
                }

                cliente.DataCadastro = DateTime.Now;

                _context.Clientes.Add(cliente);
                _context.SaveChanges();

                TempData["Mensagem"] = "Cliente Incluído com Sucesso!";
                return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao incluir Cliente no banco de dados.");
                    ModelState.AddModelError(string.Empty, "Ocorreu um erro ao salvar no banco. Tente novamente.");
                }
            }
            return View(cliente);
        }
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
        // GET: Clientes/Alterar/5
        [HttpGet]
        public IActionResult Alterar(int id)
        {
<<<<<<< HEAD
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

=======
            var cliente = _context.Clientes.FirstOrDefault(c => c.IdCliente == id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente);
        }
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
        // POST: Clientes/Alterar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Alterar(Cliente cliente)
        {
<<<<<<< HEAD
            try
            {
                _entidadesService.NormalizarCampos(cliente);

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

=======
            NormalizarDados(cliente);

            if (ModelState.IsValid)
            {
                // Verifica duplicidade ao alterar (exceto o próprio registro)
                bool existe = _context.Clientes.Any(c => c.CpfCnpj == cliente.CpfCnpj && c.IdCliente != cliente.IdCliente);
                if (existe)
                {
                    ModelState.AddModelError("CpfCnpj", "Este CPF/CNPJ já está cadastrado em outro cliente.");
                    return View(cliente);
                }

                _context.Clientes.Update(cliente);
                _context.SaveChanges();

                TempData["Mensagem"] = "Cliente Alterado com Sucesso!";
                return RedirectToAction("Index");
            }
            return View(cliente);
        }
        // GET: Clientes/Excluir/5
        public IActionResult Excluir(int id)
        {
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
            var cliente = _context.Clientes.FirstOrDefault(c => c.IdCliente == id);
            if (cliente == null)
            {
                return NotFound();
            }
            return View(cliente); // retorna view de confirmação
        }
<<<<<<< HEAD

=======
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
        // POST: Clientes/Excluir/5
        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public IActionResult ExcluirConfirmado(int id)
        {
<<<<<<< HEAD
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
=======
            var cliente = _context.Clientes.Find(id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
                _context.SaveChanges();
                TempData["Mensagem"] = "Cliente Excluído com Sucesso!";
            }
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<JsonResult> BuscarDadosPorCnpj(string cnpj)
        {
            try
            {
                using var client = new HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(20) // evita esperar 100s
                };
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                var response = await client.GetAsync($"https://www.receitaws.com.br/v1/cnpj/{cnpj}");

                if (!response.IsSuccessStatusCode)
                {
                    return Json(new { erro = true, mensagem = "Erro ao consultar CNPJ." });
                }

                var json = await response.Content.ReadAsStringAsync();

                // Usa desserialização dinâmica para não dar erro em campos desconhecidos
                var dados = System.Text.Json.JsonSerializer.Deserialize<System.Text.Json.JsonElement>(json);

                return Json(dados);
            }
            catch (TaskCanceledException)
            {
                return Json(new { erro = true, mensagem = "Tempo de consulta ao CNPJ expirou." });
            }
            catch (Exception ex)
            {
                return Json(new { erro = true, mensagem = $"Erro inesperado: {ex.Message}" });
            }
        }
        // GET: Verifica se CPF/CNPJ já Cadastrado
        [HttpGet]
        public async Task<JsonResult> VerificarCpfCnpj(string cpfcnpj)
        {
            bool existe = _context.Clientes.Any(c => c.CpfCnpj == cpfcnpj);
            return Json(new { existe });
        }
        // Função utilitária para limpar formatação
        private string SemFormatacao(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return "";
            return valor.Replace("(", "")
                        .Replace(")", "")
                        .Replace(".", "")
                        .Replace("-", "")
                        .Replace("/", "")
                        .Replace("R", "")
                        .Replace("$", "")
                        .Replace(" ", "")
                        .Trim();
        }
        // Função Normaliza Campos
        private void NormalizarDados(Cliente cliente)
        {
            cliente.CpfCnpj = SemFormatacao(cliente.CpfCnpj);
            cliente.Cep = SemFormatacao(cliente.Cep);
            cliente.Fone1 = SemFormatacao(cliente.Fone1);
            cliente.Fone2 = SemFormatacao(cliente.Fone2);
            cliente.Email = cliente.Email?.ToLower().Trim() ?? string.Empty;
            cliente.NomeRazaoSocial = cliente.NomeRazaoSocial?.ToUpper() ?? string.Empty;
            cliente.Endereco = cliente.Endereco?.ToUpper() ?? string.Empty;
            cliente.Numero = cliente.Numero?.ToUpper() ?? string.Empty;
            cliente.Bairro = cliente.Bairro?.ToUpper() ?? string.Empty;
            cliente.Municipio = cliente.Municipio?.ToUpper() ?? string.Empty;
            cliente.Uf = cliente.Uf?.ToUpper() ?? string.Empty;
            cliente.Contato = cliente.Contato?.ToUpper() ?? string.Empty;
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
        }
    }
}