using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ordem_Servicos_Web.Controllers.Cadastros;
using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Helpers;
using Ordem_Servicos_Web.Models;
using Ordem_Servicos_Web.Services;
using Ordem_Servicos_Web.Services.Interfaces;
using Serilog.Core;
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
        EntidadesService entidadesService,
        IImageService imageService) : Controller
    {
        private readonly MeuDbContext _context = context;
        private readonly ICnpjService<Cliente> _clienteCnpjService = clienteCnpjService;
        private readonly ICnpjService<Fornecedor> _fornecedorCnpjService = fornecedorCnpjService;
        private readonly EntidadesService _entidadesService = entidadesService;
        private readonly IImageService _imageService = imageService;


        /// Verifica duplicidade de CPF/CNPJ em Clientes, Fornecedores ou Usuários.
        [HttpGet]
        public async Task<IActionResult> VerificarDuplicidade(string campo, string valor, string entidade)
        {
            if (string.IsNullOrWhiteSpace(campo) || string.IsNullOrWhiteSpace(valor) || string.IsNullOrWhiteSpace(entidade))
                return Json(new { existe = false });

            bool existe = entidade.ToUpperInvariant() switch
            {
                "CLIENTES" => await _context.Clientes.AnyAsync(c => EF.Property<string>(c, campo) == valor),
                "FORNECEDORES" => await _context.Fornecedores.AnyAsync(f => EF.Property<string>(f, campo) == valor),
                "USUARIOS" => await _context.Usuarios.AnyAsync(u => EF.Property<string>(u, campo) == valor),
                _ => throw new ArgumentException("Entidade inválida.")
            };

            return Json(new { existe });
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
                {
                    return Json(new { sucesso = false});
                }

                var usuario = _context.Usuarios
                    .FirstOrDefault(u => u.Login.Equals(login, StringComparison.Ordinal));

                if (usuario != null)
                {

                    bool senhaValida = PasswordHelper.VerificarSenha(senha, usuario.Senha);

                    if (senhaValida)
                    {
                        return Json(new { sucesso = true});
                    }
                }
                return Json(new { sucesso = false});
            }
            catch (Exception)
            {
                return Json(new { sucesso = false});
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

            var query = metodoWhere.Invoke(null, new object[] { dbSet, lambda });

            var metodoFirstOrDefault = typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == "FirstOrDefault" && m.GetParameters().Length == 1)
                .MakeGenericMethod(tipoEntidade);

            var entidadeEncontrada = metodoFirstOrDefault.Invoke(null, new object[] { query });

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