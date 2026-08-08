using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Helpers;
using Ordem_Servicos_Web.Models;
using Ordem_Servicos_Web.Services.Interfaces;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;

namespace Ordem_Servicos_Web.Controllers.Entidades
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class EntidadesController(
        MeuDbContext context,
        ICnpjService<Cliente> clienteCnpjService,
        ICnpjService<Fornecedor> fornecedorCnpjService,
        IImageService imageService) : Controller
    {
        private readonly MeuDbContext _context = context;
        private readonly ICnpjService<Cliente> _clienteCnpjService = clienteCnpjService;
        private readonly ICnpjService<Fornecedor> _fornecedorCnpjService = fornecedorCnpjService;
        private readonly IImageService _imageService = imageService;

        /// Verifica duplicidade de CPF/CNPJ em Clientes, Fornecedores ou Usuários.
        [HttpGet]
        public async Task<IActionResult> VerificarDuplicidade(string entidade, [FromQuery] Dictionary<string, string> campos)
        {
            if (string.IsNullOrWhiteSpace(entidade) || campos == null || campos.Count == 0)
                return Json(new { existe = false });

            bool existe = entidade.ToUpperInvariant() switch
            {
                "CLIENTES" => await AplicarFiltros(_context.Clientes.AsQueryable(), campos),
                "FORNECEDORES" => await AplicarFiltros(_context.Fornecedores.AsQueryable(), campos),
                "USUARIOS" => await AplicarFiltros(_context.Usuarios.AsQueryable(), campos),
                "PRODUTOS" => await AplicarFiltros(_context.Produtos.AsQueryable(), campos),
                "CATEGORIASERVICOS" => await AplicarFiltros(_context.CategoriaServicos.AsQueryable(), campos),
                "SERVICOS" => await AplicarFiltros(_context.Servicos.AsQueryable(), campos),
                "UNIDADES" => await AplicarFiltros(_context.Unidades.AsQueryable(), campos),
                "MARCAS" => await AplicarFiltros(_context.Marcas.AsQueryable(), campos),
                "MODELOS" => await AplicarFiltros(_context.Modelos.AsQueryable(), campos),
                "PERMISSOES" => await AplicarFiltros(_context.Permissoes.AsQueryable(), campos),
                "LOGS" => await AplicarFiltros(_context.Logs.AsQueryable(), campos),
                _ => throw new ArgumentException("Entidade inválida.")
            };

            return Json(new { existe });
        }

        private async Task<bool> AplicarFiltros<T>(IQueryable<T> query, Dictionary<string, string> campos) where T : class
        {
            foreach (var kvp in campos)
            {
                string campo = kvp.Key;
                string valor = kvp.Value;

                var prop = typeof(T).GetProperty(campo, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (prop == null) continue;

                object? convertedValue;
                try
                {
                    var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                    convertedValue = Convert.ChangeType(valor, targetType, CultureInfo.InvariantCulture);
                }
                catch
                {
                    continue;
                }

                var parameter = Expression.Parameter(typeof(T), "e");
                var propertyAccess = Expression.Property(parameter, prop);
                var constant = Expression.Constant(convertedValue, prop.PropertyType);
                var equal = Expression.Equal(propertyAccess, constant);
                var lambda = Expression.Lambda<Func<T, bool>>(equal, parameter);

                query = query.Where(lambda);
            }

            return await query.AnyAsync();
        }

        /// Busca dados de Cliente ou Fornecedor por CNPJ.
        [HttpGet]
        public async Task<IActionResult> BuscarDadosPorCnpj(string cnpj, string entidade)
        {
            if (string.IsNullOrWhiteSpace(cnpj) || string.IsNullOrWhiteSpace(entidade))
                return Json(new { sucesso = false, mensagem = "Parâmetros inválidos" });

            // 🔹 Normaliza CNPJ (remove pontos, traços, barras)
            cnpj = FormatHelper.SomenteNumeros(cnpj);

            PessoaFisicaJuridica? dados = entidade.ToUpperInvariant() switch
            {
                "CLIENTES" => await _clienteCnpjService.ConsultarCnpjAsync(cnpj),
                "FORNECEDORES" => await _fornecedorCnpjService.ConsultarCnpjAsync(cnpj),
                _ => null
            };

            if (dados == null)
                return Json(new { sucesso = false, mensagem = "CNPJ não encontrado ou inválido" });

            return Json(new
            {
                sucesso = true,
                nomeRazaoSocial = dados.NomeRazaoSocial,
                endereco = dados.Endereco,
                numero = dados.Numero,
                bairro = dados.Bairro,
                municipio = dados.Municipio,
                uf = dados.Uf,
                cep = FormatHelper.SomenteNumeros(dados.Cep ?? ""),
                foneFixo = FormatHelper.SomenteNumeros(dados.FoneFixo ?? ""),
                foneCelular = FormatHelper.SomenteNumeros(dados.FoneCelular ?? ""),
                email = FormatHelper.ConverteParaMinusculo(dados.Email ?? ""),
                contato = dados.Contato
            });
        }


        // Valida login e senha de usuário para autenticação.
        [HttpGet]
        public IActionResult ValidarLoginSenha(string login, string senha)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(senha))
                    return Json(new { sucesso = false });

                var usuario = _context.Usuarios
                    .FirstOrDefault(u => u.Login != null && u.Login.Equals(login, StringComparison.Ordinal));

                // Garante que não passemos null para PasswordHelper.VerificarSenha
                if (usuario == null || string.IsNullOrEmpty(usuario.Senha))
                    return Json(new { sucesso = false });

                bool senhaValida = PasswordHelper.VerificarSenha(senha, usuario.Senha);

                if (senhaValida)
                    return Json(new { sucesso = true });

                return Json(new { sucesso = false });
            }
            catch (Exception)
            {
                return Json(new { sucesso = false });
            }
        }

        // Busca listas de entidades (Fornecedores, Marcas, Modelos, Unidades) para dropdowns, com opção de filtro por marca para modelos.
        [HttpGet]
        public IActionResult GetEntidades(string entidade, string campoDescricao, string apelido, int? filtroId = null)
        {
            if (string.IsNullOrWhiteSpace(entidade) || string.IsNullOrWhiteSpace(campoDescricao))
                return Json(new { sucesso = false, mensagem = "Parâmetros inválidos" });

            object resultado = entidade.ToUpperInvariant() switch
            {
                "FORNECEDORES" => _context.Fornecedores
                    .Select(apelido => new {
                        id = apelido.IdFornecedor,
                        valorDescricao = EF.Property<string>(apelido, campoDescricao),
                        apelido
                    }).ToList(),

                "MARCAS" => _context.Marcas
                    .Select(apelido => new {
                        id = apelido.IdMarca,
                        valorDescricao = EF.Property<string>(apelido, campoDescricao),
                        apelido
                    }).ToList(),


                "MODELOS" => _context.Modelos
                    .Where(apelido => filtroId == null || apelido.IdMarca == filtroId)
                    .Select(apelido => new {
                        id = apelido.IdModelo,
                        valorDescricao = EF.Property<string>(apelido, campoDescricao),
                        apelido
                    }).ToList(),

                "UNIDADES" => _context.Unidades
                    .Select(apelido => new {
                        id = apelido.IdUnidade,
                        valorDescricao = EF.Property<string>(apelido, campoDescricao),
                        apelido
                    }).ToList(),

                "CATEGORIASERVICOS" => _context.CategoriaServicos
                    .Select(apelido => new {
                        id = apelido.IdCategoriaServico,
                        valorDescricao = EF.Property<string>(apelido, campoDescricao),
                        apelido
                    }).ToList(),

                "USUARIOS" => _context.Usuarios
                    .Select(apelido => new {
                        id = apelido.IdUsuario,
                        valorDescricao = EF.Property<string>(apelido, campoDescricao),
                        apelido
                    }).ToList(),

                "MENUS" => _context.Menus
                    .Select(apelido => new {
                        id = apelido.IdMenu,
                        valorDescricao = EF.Property<string>(apelido, campoDescricao),
                        apelido
                    }).ToList(),

                "ITENSMENUS" => _context.ItensMenus
                    .Where(apelido => filtroId == null || apelido.IdMenu == filtroId)
                    .Select(apelido => new {
                        id = apelido.IdItensMenu,
                        valorDescricao = EF.Property<string>(apelido, campoDescricao),
                        apelido
                    }).ToList(),

                _ => throw new ArgumentException("Entidade inválida.")
            };

            return Json(new { sucesso = true, dados = resultado });
        }

        [HttpGet]
        public IActionResult GetModelosPorMarca(int idMarca)
        {
            var modelos = _context.Modelos
                .Where(m => m.IdMarca == idMarca)
                .Select(m => new
                {
                    m.IdModelo,
                    m.Descricao
                })
                .ToList();

            return Json(modelos);
        }

        public JsonResult GetItensMenuPorMenu(int idMenu)
        {
            var itens = _context.ItensMenus
                .Where(i => i.IdMenu == idMenu)
                .Select(i => new
                {
                    i.IdItensMenu,
                    i.Descricao
                })
                .ToList();

            return Json(itens);
        }

        [HttpGet]
        public async Task<IActionResult> GetImagens(
            string campoID,
            string campoBD,
            string campoImagem,
            string campoDescricao,
            string entidade,
            string apelido)
        {
            if (string.IsNullOrWhiteSpace(entidade) || string.IsNullOrWhiteSpace(campoID))
                return Json(new { exists = false, mensagem = "Parâmetros inválidos" });

            var tipoEntidade = Assembly.GetExecutingAssembly()
                .GetTypes()
                .FirstOrDefault(t => t.Name.Equals(entidade, StringComparison.OrdinalIgnoreCase));

            if (tipoEntidade == null)
                return Json(new { exists = false, mensagem = $"Entidade '{entidade}' não encontrada." });

            // Usa reflexão para chamar _context.Set<T>()
            var metodoSet = typeof(DbContext)
                .GetMethod(nameof(DbContext.Set), Type.EmptyTypes)
                ?.MakeGenericMethod(tipoEntidade);

            var dbSet = metodoSet?.Invoke(_context, null) as IQueryable<object>;
            if (dbSet == null)
                return Json(new { exists = false, mensagem = $"DbSet para '{entidade}' não encontrado." });

            // Monta filtro dinamicamente: x => x.campoBD == campoID
            var parametro = Expression.Parameter(tipoEntidade, apelido);
            var propriedadeCampoBD = Expression.Property(parametro, campoBD);
            var valorCampoID = Convert.ChangeType(campoID, propriedadeCampoBD.Type);
            var igualdade = Expression.Equal(propriedadeCampoBD, Expression.Constant(valorCampoID));
            var lambda = Expression.Lambda(igualdade, parametro);

            var metodoWhere = typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == "Where" && m.GetParameters().Length == 2)
                .MakeGenericMethod(tipoEntidade);

            var query = metodoWhere.Invoke(null, [dbSet, lambda]);

            var metodoFirstOrDefault = typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == "FirstOrDefault" && m.GetParameters().Length == 1)
                .MakeGenericMethod(tipoEntidade);

            var entidadeEncontrada = metodoFirstOrDefault.Invoke(null, [query]);

            if (entidadeEncontrada == null)
                return Json(new { exists = false, mensagem = "Registro não encontrado." });

            // Obtém imagem e descrição
            var propImagem = tipoEntidade.GetProperty(campoImagem);
            var propDescricao = tipoEntidade.GetProperty(campoDescricao);
            var propBD = tipoEntidade.GetProperty(campoBD);

            var imagemBytes = propImagem?.GetValue(entidadeEncontrada) as byte[] ?? Array.Empty<byte>();
            var descricao = propDescricao?.GetValue(entidadeEncontrada)?.ToString() ?? string.Empty;
            var codigoBD = propBD?.GetValue(entidadeEncontrada)?.ToString() ?? string.Empty;

            var resultado = _imageService.ProcessarImagem(imagemBytes, descricao, codigoBD);

            return Json(resultado);
        }

    }
}