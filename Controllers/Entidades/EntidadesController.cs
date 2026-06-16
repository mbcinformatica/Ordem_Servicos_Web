using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ordem_Servicos_Web.Controllers.Cadastros;
using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Helpers;
using Ordem_Servicos_Web.Models;
using Ordem_Servicos_Web.Services;
using Ordem_Servicos_Web.Services.Interfaces;
using Serilog.Core;

namespace Ordem_Servicos_Web.Controllers.Entidades
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class EntidadesController(
        MeuDbContext context,
        ICnpjService<Cliente> clienteCnpjService,
        ICnpjService<Fornecedor> fornecedorCnpjService,
        EntidadesService entidadesService) : Controller
    {
        private readonly MeuDbContext _context = context;
        private readonly ICnpjService<Cliente> _clienteCnpjService = clienteCnpjService;
        private readonly ICnpjService<Fornecedor> _fornecedorCnpjService = fornecedorCnpjService;
        private readonly EntidadesService _entidadesService = entidadesService;


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

    }
}