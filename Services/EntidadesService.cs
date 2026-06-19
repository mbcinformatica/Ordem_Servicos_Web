using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Helpers;

namespace Ordem_Servicos_Web.Services
{
    public class EntidadesService(MeuDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        private readonly MeuDbContext _context = context;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public void NormalizarCampos(object entidade, IEnumerable<string> campos)
        {
            if (campos == null || entidade == null)
            {
                return;
            }

            foreach (var campo in campos)
            {
                var propriedade = entidade.GetType().GetProperty(campo);
                if (propriedade == null) continue;

                var valor = propriedade.GetValue(entidade);
                if (valor == null) continue;

                if (valor is string str)
                {
                    propriedade.SetValue(entidade, FormatHelper.SemFormatacao(str));
                }
            }
        }
    }
}