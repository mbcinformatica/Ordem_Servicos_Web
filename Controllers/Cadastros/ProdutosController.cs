using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Helpers;
using Ordem_Servicos_Web.Models;
using Ordem_Servicos_Web.Services;
using Ordem_Servicos_Web.ViewModels;

namespace Ordem_Servicos_Web.Controllers.Cadastros
{
    public class ProdutosController(MeuDbContext context, ILogger<ProdutosController> logger, PermissaoService permissaoService, EntidadesService entidadesService) : Controller
    {
        private readonly MeuDbContext _context = context;
        private readonly ILogger<ProdutosController> _logger = logger;
        private readonly PermissaoService _permissaoService = permissaoService;
        private readonly EntidadesService _entidadesService = entidadesService;

        // Index com paginação + pesquisa
        public IActionResult Index(int page = 1, string search = "", string column = "Descricao")
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);

            if (!_permissaoService.PodeExecutar(idUsuario, "CADASTROS", "PRODUTOS"))
            {
                TempData["Mensagem"] = "Você não tem permissão para acessar essa tela.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index", "Home");
            }

            int pageSize = 10;

            var query = _context.Produtos
                .Include(pr => pr.Fornecedor)
                .Include(pr => pr.Marca)
                .Include(pr => pr.Modelo)
                .Include(pr => pr.Unidade)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = ApplySearchFilter(query, search, column);
            }

            query = ApplyOrdering(query, column);

            var produtos = query
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

            return View(produtos);
        }

        // Action para pesquisa Ajax
        public IActionResult Search(string search = "", string column = "Descricao")
        {
            var query = _context.Produtos
                .Include(pr => pr.Fornecedor)
                .Include(pr => pr.Marca)
                .Include(pr => pr.Modelo)
                .Include(pr => pr.Unidade)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = ApplySearchFilter(query, search, column);
            }

            query = ApplyOrdering(query, column);

            var produtos = query.ToList();

            return PartialView("_ProdutosTable", produtos);
        }

        // Método auxiliar para aplicar filtro
        private static IQueryable<Produto> ApplySearchFilter(IQueryable<Produto> query, string search, string column)
        {
            switch (column)
            {
                case "IdProduto":
                    if (int.TryParse(search, out int id))
                        query = query.Where(pr => pr.IdProduto == id);
                    break;
                case "IdProdutoInterno":
                    query = query.Where(pr => pr.IdProdutoInterno.StartsWith(search, StringComparison.CurrentCultureIgnoreCase));
                    break;
                case "IdProdutoFabricante":
                    query = query.Where(pr => pr.IdProdutoFabricante.StartsWith(search, StringComparison.CurrentCultureIgnoreCase));
                    break;
                case "FornecedorNome":
                    query = query.Where(pr => pr.Fornecedor.NomeRazaoSocial.StartsWith(search, StringComparison.CurrentCultureIgnoreCase));
                    break;
                default: // Descricao
                    query = query.Where(pr => pr.Descricao.StartsWith(search, StringComparison.CurrentCultureIgnoreCase));
                    break;
            }
            return query;
        }

        // Método auxiliar para aplicar ordenação
        private static IQueryable<Produto> ApplyOrdering(IQueryable<Produto> query, string column) => column switch
        {
            "IdProduto" => query.OrderBy(pr => pr.IdProduto),
            "IdProdutoInterno" => query.OrderBy(pr => pr.IdProdutoInterno),
            "IdProdutoFabricante" => query.OrderBy(pr => pr.IdProdutoFabricante),
            "Descricao" => query.OrderBy(pr => pr.Descricao),
            _ => query.OrderBy(pr => pr.IdFornecedor),
        };

        // GET: Produtos/Create
        public IActionResult Create()
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeCriar(idUsuario, "CADASTROS", "PRODUTOS"))
            {
                TempData["Mensagem"] = "Você não tem permissão para executar essa ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }

            var model = new ProdutoViewModel();

            return View(model);
        }

        // POST: Produtos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProdutoViewModel model, IFormFile Imagem)
        {
            try
            {
                var normalizarCampos = new[] { "PrecoCompra", "PrecoVenda" };
                _entidadesService.NormalizarCampos(model, normalizarCampos);

                if (ModelState.IsValid)
                {

                    byte[]? imagemBytes = null;
                    if (Imagem != null && Imagem.Length > 0)
                    {
                        using var ms = new MemoryStream();
                        Imagem.CopyTo(ms);
                        imagemBytes = ms.ToArray();
                    }

                    var produto = new Produto
                    {
                        IdProduto = model.IdProduto,
                        IdProdutoInterno = model.IdProdutoInterno,
                        IdProdutoFabricante = model.IdProdutoFabricante,
                        //Descricao = model.Descricao,
                        Descricao = model.Modelo?.Descricao ?? "Não informado",
                        IdFornecedor = model.IdFornecedor,
                        IdMarca = model.IdMarca,
                        IdModelo = model.IdModelo,
                        IdUnidade = model.IdUnidade,
                        PrecoCompra = model.PrecoCompra,
                        PrecoVenda = model.PrecoVenda,
                        EstoqueAtual = model.EstoqueAtual,
                        EstoqueMinimo = model.EstoqueMinimo,
                        Imagem = imagemBytes
                    };

                    _context.Produtos.Add(produto);
                    _context.SaveChanges();

                    TempData["Mensagem"] = "Produto incluído com sucesso!";
                    TempData["MensagemTipo"] = "sucesso";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Mensagem"] = "Erro na validação das informações. Tente novamente.";
                    TempData["MensagemTipo"] = "aviso";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao incluir produto no banco de dados.");
                TempData["Mensagem"] = "Erro ao salvar no banco de dados. Tente novamente.";
                TempData["MensagemTipo"] = "erro";
            }

            return View(model);
        }

        // GET: Produtos/Alterar/5
        [HttpGet]
        public IActionResult Alterar(int id)
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeAlterar(idUsuario, "CADASTROS", "PRODUTOS"))
            {
                TempData["Mensagem"] = "Você não tem permissão para executar essa ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }


            var produto = _context.Produtos
                .Include(pr => pr.Fornecedor)
                .Include(pr => pr.Marca)
                .Include(pr => pr.Modelo)
                .Include(pr => pr.Unidade)
                .FirstOrDefault(pr => pr.IdProduto == id);
            
            if (produto == null)
            {
                return NotFound();
            }

            var model = new ProdutoViewModel
            {
                IdProduto = produto.IdProduto,
                IdProdutoInterno = produto.IdProdutoInterno,
                IdProdutoFabricante = produto.IdProdutoFabricante,
                Descricao = produto.Descricao,
                IdFornecedor = produto.IdFornecedor,
                IdMarca = produto.IdMarca,
                IdModelo = produto.IdModelo,
                IdUnidade = produto.IdUnidade,
                PrecoCompra = produto.PrecoCompra,
                PrecoVenda = produto.PrecoVenda,
                EstoqueAtual = produto.EstoqueAtual,
                EstoqueMinimo = produto.EstoqueMinimo,
                Imagem = produto.Imagem,

                // Aqui você pode expor os nomes já carregados
                NomeFornecedor = produto.Fornecedor?.NomeRazaoSocial,
                NomeMarca = produto.Marca?.Descricao,
                NomeModelo = produto.Modelo?.Descricao,
                NomeUnidade = produto.Unidade?.Descricao
            };


            return View(model);
        }

        // POST: Produtos/Alterar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Alterar(ProdutoViewModel model, IFormFile Imagem, string ImagemBase64)
        {
            try

            {
                var normalizarCampos = new[] { "PrecoCompra", "PrecoVenda" };
                _entidadesService.NormalizarCampos(model, normalizarCampos);

                ModelState.Remove("Imagem");
                ModelState.Remove("ImagemBase64");

                if (!ModelState.IsValid)
                {
                    TempData["Mensagem"] = "Ocorreu um Erro na Validação das Informações. Tente Novamente.";
                    TempData["MensagemTipo"] = "erro";
                    return View(model);
                }

                var produtoDb = _context.Produtos.Find(model.IdProduto);
                if (produtoDb == null) return NotFound();

                produtoDb.IdProdutoInterno = model.IdProdutoInterno;
                produtoDb.IdProdutoFabricante = model.IdProdutoFabricante;
                produtoDb.Descricao = model.Descricao;
                produtoDb.IdFornecedor = model.IdFornecedor;
                produtoDb.IdMarca = model.IdMarca;
                produtoDb.IdModelo = model.IdModelo;
                produtoDb.IdUnidade = model.IdUnidade;
                produtoDb.PrecoCompra = model.PrecoCompra;
                produtoDb.PrecoVenda = model.PrecoVenda;
                produtoDb.EstoqueAtual = model.EstoqueAtual;
                produtoDb.EstoqueMinimo = model.EstoqueMinimo;

                // Atualiza imagem
                if (Imagem != null && Imagem.Length > 0)
                {
                    using var ms = new MemoryStream();
                    Imagem.CopyTo(ms);
                    produtoDb.Imagem = ms.ToArray();
                }
                else if (!string.IsNullOrEmpty(ImagemBase64))
                {
                    produtoDb.Imagem = Convert.FromBase64String(ImagemBase64);
                }
                 _context.Produtos.Update(produtoDb);
                 _context.SaveChanges();

                 TempData["Mensagem"] = "Produto alterado com sucesso!";
                 TempData["MensagemTipo"] = "sucesso";
                 return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao alterar produto no banco de dados.");
                TempData["Mensagem"] = "Erro ao salvar no banco de dados. Tente novamente.";
                TempData["MensagemTipo"] = "erro";
            }
            return View(model);
        }

        // GET: Produtos/Excluir/5
        public IActionResult Excluir(int id)
        {
            var idUsuario = UsuarioSessaoHelper.ObterUsuarioLogado(HttpContext);
            if (!_permissaoService.PodeExcluir(idUsuario, "CADASTROS", "PRODUTOS"))
            {
                TempData["Mensagem"] = "Você não tem permissão para executar essa ação.";
                TempData["MensagemTipo"] = "aviso";
                return RedirectToAction("Index");
            }

            var produto = _context.Produtos.FirstOrDefault(pr => pr.IdProduto == id);
            if (produto == null)
            {
                return NotFound();
            }
            return View(produto);            
        }

        // POST: Produtos/Excluir/5
        [HttpPost, ActionName("Excluir")]
        [ValidateAntiForgeryToken]
        public IActionResult ExcluirConfirmado(int id)
        {
            try
            {
                var produto = _context.Produtos.Find(id);
                if (produto != null)
                {
                    _context.Produtos.Remove(produto);
                    _context.SaveChanges();
                    TempData["Mensagem"] = "Produto excluído com sucesso!";
                    TempData["MensagemTipo"] = "sucesso";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir produto no banco de dados.");
                TempData["Mensagem"] = "Ocorreu um erro ao excluir no banco de dados. Tente novamente.";
                TempData["MensagemTipo"] = "erro";
            }
            return RedirectToAction("Index");
        }

        // GET: Produtos/Detalhes/5
        [HttpGet]
        public IActionResult Detalhes(int id)
        {
            var produto = _context.Produtos
                .Include(pr => pr.Fornecedor)
                .Include(pr => pr.Marca)
                .Include(pr => pr.Modelo)
                .Include(pr => pr.Unidade)
                .FirstOrDefault(pr => pr.IdProduto == id);

            if (produto == null)
            {
                return NotFound();
            }

            var model = new ProdutoViewModel
            {
                IdProduto = produto.IdProduto,
                IdProdutoInterno = produto.IdProdutoInterno,
                IdProdutoFabricante = produto.IdProdutoFabricante,
                Descricao = produto.Descricao,
                PrecoCompra = produto.PrecoCompra,
                PrecoVenda = produto.PrecoVenda,
                EstoqueAtual = produto.EstoqueAtual,
                EstoqueMinimo = produto.EstoqueMinimo,
                Imagem = produto.Imagem,

                // nomes já resolvidos
                NomeFornecedor = produto.Fornecedor?.NomeRazaoSocial ?? "Não informado",
                NomeMarca = produto.Marca?.Descricao ?? "Não informado",
                NomeModelo = produto.Modelo?.Descricao ?? "Não informado",
                NomeUnidade = produto.Unidade?.Descricao ?? "Não informado"
            };

            return View(model);
        }
    }
}