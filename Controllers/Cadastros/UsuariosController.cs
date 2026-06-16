using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Helpers;
using Ordem_Servicos_Web.Models;
using Ordem_Servicos_Web.Services;
using Ordem_Servicos_Web.Services.Interfaces;
using Ordem_Servicos_Web.ViewModels;


namespace Ordem_Servicos_Web.Controllers.Cadastros
{
    public class UsuariosController(MeuDbContext context, ILogger<UsuariosController> logger, PermissaoService permissaoService, EntidadesService entidadesService, IImageService imageService) : Controller
    {
        private readonly MeuDbContext _context = context;
        private readonly ILogger<UsuariosController> _logger = logger;
        private readonly PermissaoService _permissaoService = permissaoService;
        private readonly EntidadesService _entidadesService = entidadesService;
        private readonly IImageService _imageService = imageService;

        // 🔹 Index: apenas ADMINISTRADOR pode listar usuários
        public IActionResult Index(int page = 1, string search = "", string column = "NomeUsuario")
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);

            if (!_permissaoService.PodeExecutar(idUsuario, "CADASTROS", "USUARIOS"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Acessar essa Tela.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index", "Home");
            }

            int pageSize = 10;

=======
using Microsoft.EntityFrameworkCore;
using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Models;
using Ordem_Servicos_Web.ViewModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ordem_Servicos_Web.Controllers.Cadastros
{
    public class UsuariosController : Controller
    {
        private readonly MeuDbContext _context;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(MeuDbContext context, ILogger<UsuariosController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Index com paginação + pesquisa
        public IActionResult Index(int page = 1, string search = "")
        {
            int pageSize = 10;
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
            var query = _context.Usuarios.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
<<<<<<< HEAD
                query = ApplySearchFilter(query, search, column);
            }

            query = ApplyOrdering(query, column);
=======
                query = query.Where(u => u.NomeUsuario.StartsWith(search));
            }

            query = query.OrderBy(u => u.NomeUsuario);
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7

            var usuarios = query
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
=======
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7

            return View(usuarios);
        }

        // Action para pesquisa Ajax
<<<<<<< HEAD
        public IActionResult Search(string search = "", string column = "NomeUsuario")
=======
        public IActionResult Search(string search = "")
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
        {
            var query = _context.Usuarios.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
<<<<<<< HEAD
                query = ApplySearchFilter(query, search, column);
            }

            query = ApplyOrdering(query, column);
=======
                query = query.Where(u => u.NomeUsuario.StartsWith(search));
            }

            query = query.OrderBy(u => u.NomeUsuario);
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7

            var usuarios = query.ToList();

            return PartialView("_UsuariosTable", usuarios);
        }

<<<<<<< HEAD
        // Método auxiliar para aplicar filtro
        private static IQueryable<Usuario> ApplySearchFilter(IQueryable<Usuario> query, string search, string column)
        {
            switch (column)
            {
                case "IdUsuario":
                    if (int.TryParse(search, out int id))
                        query = query.Where(us => us.IdUsuario == id);
                    break;
                case "Login":
                    query = query.Where(us => us.Login.StartsWith(search));
                    break;
                default: // Nome Usuário
                    query = query.Where(us => us.NomeUsuario.StartsWith(search));
                    break;
            }
            return query;
        }

        // Método auxiliar para aplicar ordenação
        private static IQueryable<Usuario> ApplyOrdering(IQueryable<Usuario> query, string column)
        {
            return column switch
            {
                "IdUsuario" => query.OrderBy(us => us.IdUsuario),
                "Login" => query.OrderBy(us => us.Login),
                _ => query.OrderBy(us => us.NomeUsuario),
            };
        }

        // GET: Usuarios/Create Criar: apenas quem tem permissão "Criar"
        public IActionResult Create()
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeCriar(idUsuario, "CADASTROS", "USUÁRIOS"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Executar essa Ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }

=======
        // GET: Usuarios/Create
        public IActionResult Create()
        {
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
<<<<<<< HEAD
        public IActionResult Create(UsuarioViewModel model, IFormFile Imagem)
        {
            try
            {
                _entidadesService.NormalizarDadosUsuario(model);

                if (ModelState.IsValid)
                {
=======
        public IActionResult Create(UsuarioCreateViewModel model, IFormFile Imagem)
        {
            NormalizarDados(model);

            if (ModelState.IsValid)
            {
                try
                {
                    bool existe = _context.Usuarios.Any(u => u.Login == model.Login);
                    if (existe)
                    {
                        ModelState.AddModelError("Login", "Login já Cadastrado.");
                        return View(model);
                    }
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7

                    byte[]? imagemBytes = null;
                    if (Imagem != null && Imagem.Length > 0)
                    {
<<<<<<< HEAD
                        using var ms = new MemoryStream();
                        Imagem.CopyTo(ms);
                        imagemBytes = ms.ToArray();
=======
                        using (var ms = new MemoryStream())
                        {
                            Imagem.CopyTo(ms);
                            imagemBytes = ms.ToArray();
                        }
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
                    }

                    var usuario = new Usuario
                    {
                        NomeUsuario = model.NomeUsuario,
                        Login = model.Login,
<<<<<<< HEAD
                        Senha = PasswordHelper.HashPasswordBCrypt(model.Senha ?? string.Empty),
                        Email = model.Email,
                        Cep = model.Cep,
                        FoneFixo = model.FoneFixo,
                        FoneCelular = model.FoneCelular,
=======
                        Senha = PasswordHelper.HashPassword(model.Senha),
                        Email = model.Email,
                        Cep = model.Cep,
                        Fone1 = model.Fone1,
                        Fone2 = model.Fone2,
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
                        Endereco = model.Endereco,
                        Numero = model.Numero,
                        Bairro = model.Bairro,
                        Municipio = model.Municipio,
                        Uf = model.Uf,
                        DataCadastro = DateTime.Now,
<<<<<<< HEAD
                        Imagem = imagemBytes
=======
                        Imagem = imagemBytes // salva a foto no BD
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
                    };

                    _context.Usuarios.Add(usuario);
                    _context.SaveChanges();

                    TempData["Mensagem"] = "Usuário Incluído com Sucesso!";
<<<<<<< HEAD
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
                _logger.LogError(ex, "Erro ao incluir usuário no banco de dados.");
                TempData["Mensagem"] = "Ocorreu um Erro ao Salvar Usuário no Banco de Dados. Tente Novamente.";
            }
            TempData["MensagemTipo"] = "erro";
            return View(model);
        }

        // GET: Usuarios/Alterar Alterar: apenas quem tem permissão "Alterar"
        public IActionResult Alterar(int id)
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeAlterar(idUsuario, "CADASTROS", "USUÁRIOS"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Executar essa Ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }

            var usuario = _context.Usuarios.Find(id);
            if (usuario == null) return NotFound();

            var model = new UsuarioViewModel
=======
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao incluir usuário no banco de dados.");
                    ModelState.AddModelError(string.Empty, "Ocorreu um erro ao salvar no banco. Tente novamente.");
                }
            }
            return View(model);
        }

        // GET: Usuarios/Alterar/5
        public IActionResult Alterar(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null) return NotFound();

            var model = new UsuarioEditViewModel
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
            {
                IdUsuario = usuario.IdUsuario,
                NomeUsuario = usuario.NomeUsuario,
                Login = usuario.Login,
                Email = usuario.Email,
                Cep = usuario.Cep,
<<<<<<< HEAD
                FoneFixo = usuario.FoneFixo,
                FoneCelular = usuario.FoneCelular,
=======
                Fone1 = usuario.Fone1,
                Fone2 = usuario.Fone2,
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
                Endereco = usuario.Endereco,
                Numero = usuario.Numero,
                Bairro = usuario.Bairro,
                Municipio = usuario.Municipio,
                Uf = usuario.Uf,
                DataCadastro = usuario.DataCadastro,
                Imagem = usuario.Imagem
            };

            return View(model);
        }

<<<<<<< HEAD
        // GET: Usuarios/Alterar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Alterar(UsuarioViewModel model, IFormFile Imagem, string ImagemBase64)
        {
            try
            {
                _entidadesService.NormalizarDadosUsuario(model);

                ModelState.Remove("Imagem");
                if (!ModelState.IsValid)
                {

                    var usuarioDb = _context.Usuarios.Find(model.IdUsuario);
                    if (usuarioDb == null) return NotFound();

                    // Atualiza campos básicos
                    usuarioDb.NomeUsuario = model.NomeUsuario;
                    usuarioDb.Login = model.Login;
                    usuarioDb.Email = model.Email;
                    usuarioDb.Cep = model.Cep;
                    usuarioDb.FoneFixo = model.FoneFixo;
                    usuarioDb.FoneCelular = model.FoneCelular;
                    usuarioDb.Endereco = model.Endereco;
                    usuarioDb.Numero = model.Numero;
                    usuarioDb.Bairro = model.Bairro;
                    usuarioDb.Municipio = model.Municipio;
                    usuarioDb.Uf = model.Uf;

                    // Atualiza senha se informada
                    if (!string.IsNullOrWhiteSpace(model.Senha))
                        usuarioDb.Senha = PasswordHelper.HashPasswordBCrypt(model.Senha);

                    // Atualiza imagem
                    if (Imagem != null && Imagem.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        Imagem.CopyTo(ms);
                        usuarioDb.Imagem = ms.ToArray();
                    }
                    else if (!string.IsNullOrEmpty(ImagemBase64))
                    {
                        usuarioDb.Imagem = Convert.FromBase64String(ImagemBase64);
                    }
                    // Se não vier nada, mantém usuarioDb.Imagem como está

                    _context.Update(usuarioDb);
                    _context.SaveChanges();

                    TempData["Mensagem"] = "Usuário Alterado com Sucesso!";
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
                _logger.LogError(ex, "Erro ao Alterar Dados do Uuário no Banco de Dados.");
                TempData["Mensagem"] = "Erro ao Alterar Dados do Uuário no Banco de Dados.";
            }
            TempData["MensagemTipo"] = "erro";
            return View(model);
        }

        // 🔹 Excluir: apenas quem tem permissão "Excluir"
        public IActionResult Excluir(int id)
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeExcluir(idUsuario, "CADASTROS", "USUÁRIOS"))
            {
                TempData["Mensagem"] = "Você não tem Permissão para Executar essa Ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }

            var usuario = _context.Usuarios.Find(id);
            if (usuario == null) return NotFound();

            return View(usuario); // retorna view de confirmação
        }

        // POST: Usuarios/Excluir
=======
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Alterar(UsuarioEditViewModel model, IFormFile Imagem, string ImagemBase64)
        {
            ModelState.Remove("Imagem");
            if (!ModelState.IsValid)
                return View(model);

            var usuarioDb = _context.Usuarios.Find(model.IdUsuario);
            if (usuarioDb == null) return NotFound();

            // Atualiza campos básicos
            usuarioDb.NomeUsuario = model.NomeUsuario;
            usuarioDb.Login = model.Login;
            usuarioDb.Email = model.Email;
            usuarioDb.Cep = model.Cep;
            usuarioDb.Fone1 = model.Fone1;
            usuarioDb.Fone2 = model.Fone2;
            usuarioDb.Endereco = model.Endereco;
            usuarioDb.Numero = model.Numero;
            usuarioDb.Bairro = model.Bairro;
            usuarioDb.Municipio = model.Municipio;
            usuarioDb.Uf = model.Uf;

            // Atualiza senha se informada
            if (!string.IsNullOrWhiteSpace(model.Senha))
                usuarioDb.Senha = PasswordHelper.HashPassword(model.Senha);

            // Atualiza imagem
            if (Imagem != null && Imagem.Length > 0)
            {
                using var ms = new MemoryStream();
                Imagem.CopyTo(ms);
                usuarioDb.Imagem = ms.ToArray();
            }
            else if (!string.IsNullOrEmpty(ImagemBase64))
            {
                usuarioDb.Imagem = Convert.FromBase64String(ImagemBase64);
            }
            // Se não vier nada, mantém usuarioDb.Imagem como está

            _context.Update(usuarioDb);
            _context.SaveChanges();

            TempData["Mensagem"] = "Usuário alterado com sucesso!";
            return RedirectToAction("Index");
        }

        // GET: Usuarios/Excluir/5
        public IActionResult Excluir(int id)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario); // retorna view de confirmação
        }

        // POST: Usuarios/Excluir/5
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public IActionResult ExcluirConfirmado(int id)
        {
            try
            {
                var usuario = _context.Usuarios.Find(id);
                if (usuario != null)
                {
<<<<<<< HEAD
                    // 🔹 Busca todas as permissões relacionadas ao usuário
                    var permissoes = _context.Permissoes
                        .Where(p => p.IdUsuario == id)
                        .ToList();

                    if (permissoes.Count != 0)
                    {
                        _context.Permissoes.RemoveRange(permissoes);
                    }

                    _context.Usuarios.Remove(usuario);
                    _context.SaveChanges();
                    TempData["Mensagem"] = "Usuário e Permissões Excluídos com Sucesso!";
                    TempData["MensagemTipo"] = "sucesso";
                }
                else
                {
                    TempData["Mensagem"] = "Usuário Não Encontrado. Não foi Possível Excluir.";
                    TempData["MensagemTipo"] = "erro";
=======
                    _context.Usuarios.Remove(usuario);
                    _context.SaveChanges();
                    TempData["Mensagem"] = "Usuário excluído com sucesso!";
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
                }
            }
            catch (Exception ex)
            {
<<<<<<< HEAD
                _logger.LogError(ex, "Erro ao Excluir Usuário no Banco de Dados.");
                TempData["Mensagem"] = "Não foi Possível Excluir o Usuário. Tente Novamente.";
                TempData["MensagemTipo"] = "erro";
            }
            return RedirectToAction("Index");
        }

        // GET: Usuarios/Detalhes
        public IActionResult Detalhes(int id)
        {
            var usuario = _context.Usuarios.Find(id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // GET: Nova Action para retornar objeto JSON com dados do usuário
        [HttpGet]
        public IActionResult GetUserImage(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return Json(new { exists = false });

            var usuario = _context.Usuarios.FirstOrDefault(u => u.Login == valor);
            if (usuario == null)
                return Json(new { exists = false });

            return Json(_imageService.ProcessarImagem(usuario.Imagem ?? [], usuario.NomeUsuario, usuario.Login));

=======
                _logger.LogError(ex, "Erro ao excluir usuário no banco de dados.");
                TempData["MensagemErro"] = "Não foi possível excluir o usuário. Tente novamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Usuarios/Detalhes/5
        public IActionResult Detalhes(int id)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // NOVA ACTION: Verificar NomeUsuario via AJAX
        [HttpGet]
        public async Task<JsonResult> VerificarNomeUsuario(string nome)
        {
            bool existe = await _context.Usuarios.AnyAsync(u => u.NomeUsuario == nome);
            return Json(new { existe });
        }
        // NOVA ACTION: Verificar Login via AJAX
        [HttpGet]
        public async Task<JsonResult> VerificarLogin(string login)
        {
            bool existe = await _context.Usuarios.AnyAsync(u => u.Login == login);
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

        private void NormalizarDados(UsuarioCreateViewModel model)
        {
            model.NomeUsuario = model.NomeUsuario?.ToUpper() ?? string.Empty;
            model.Cep = SemFormatacao(model.Cep);
            model.Fone1 = SemFormatacao(model.Fone1);
            model.Fone2 = SemFormatacao(model.Fone2);
            model.Email = model.Email?.ToLower().Trim() ?? string.Empty;
            model.Login = model.Login?.ToString() ?? string.Empty;
            model.Endereco = model.Endereco?.ToUpper() ?? string.Empty;
            model.Numero = model.Numero?.ToUpper() ?? string.Empty;
            model.Bairro = model.Bairro?.ToUpper() ?? string.Empty;
            model.Municipio = model.Municipio?.ToUpper() ?? string.Empty;
            model.Uf = model.Uf?.ToUpper() ?? string.Empty;
        }

        private void NormalizarDados(UsuarioEditViewModel model)
        {
            model.NomeUsuario = model.NomeUsuario?.ToUpper() ?? string.Empty;
            model.Cep = SemFormatacao(model.Cep);
            model.Fone1 = SemFormatacao(model.Fone1);
            model.Fone2 = SemFormatacao(model.Fone2);
            model.Email = model.Email?.ToLower().Trim() ?? string.Empty;
            model.Login = model.Login?.ToString() ?? string.Empty;
            model.Endereco = model.Endereco?.ToUpper() ?? string.Empty;
            model.Numero = model.Numero?.ToUpper() ?? string.Empty;
            model.Bairro = model.Bairro?.ToUpper() ?? string.Empty;
            model.Municipio = model.Municipio?.ToUpper() ?? string.Empty;
            model.Uf = model.Uf?.ToUpper() ?? string.Empty;
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
        }
    }
}