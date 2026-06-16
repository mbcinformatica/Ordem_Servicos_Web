using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Helpers;
using Ordem_Servicos_Web.Models;
using Ordem_Servicos_Web.ViewModels;

namespace Ordem_Servicos_Web.Services
{
    public class EntidadesService(MeuDbContext context)
    {
        private readonly MeuDbContext _context = context;

        /// Normaliza campos comuns de Pessoa Física/Jurídica.
        public void NormalizarCampos(PessoaFisicaJuridica pessoa)
        {
            if (pessoa == null) return;

            pessoa.CpfCnpj = FormatHelper.SemFormatacao(pessoa.CpfCnpj);
            pessoa.Cep = FormatHelper.SemFormatacao(pessoa.Cep);
            pessoa.FoneFixo = FormatHelper.SemFormatacao(pessoa.FoneFixo ?? string.Empty);
            pessoa.FoneCelular = FormatHelper.SemFormatacao(pessoa.FoneCelular ?? string.Empty);
            pessoa.Email = FormatHelper.ConverteParaMinusculo(pessoa.Email ?? string.Empty);
        }

        /// Normaliza campos específicos de Usuário.
        public void NormalizarDadosUsuario(UsuarioViewModel usuario)
        {
            usuario.Cep = FormatHelper.SemFormatacao(usuario.Cep);
            usuario.FoneFixo = FormatHelper.SemFormatacao(usuario.FoneFixo ?? string.Empty);
            usuario.FoneCelular = FormatHelper.SemFormatacao(usuario.FoneCelular ?? string.Empty);
            usuario.Email = FormatHelper.ConverteParaMinusculo(usuario.Email ?? string.Empty);
        }

    }
}