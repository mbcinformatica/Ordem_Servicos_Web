using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Helpers;
using Ordem_Servicos_Web.Models;
using System.Globalization;

namespace Ordem_Servicos_Web.Services
{
    public class EntidadesService(MeuDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        private readonly MeuDbContext _context = context;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        /// <summary>
        /// Normaliza campos comuns de uma entidade (remove formatação ou converte para minúsculo).
        /// </summary>
        public void NormalizarCampos(object entidade, IEnumerable<string> campos)
        {
            if (entidade == null) return;

            var tipo = entidade.GetType();
            var form = _httpContextAccessor.HttpContext?.Request.Form;

            foreach (var id in campos)
            {
                var prop = tipo.GetProperty(id);
                if (prop != null && prop.CanWrite)
                {
                    var rawValue = form?[id].ToString() ?? prop.GetValue(entidade)?.ToString() ?? string.Empty;

                    // 🔹 Email → minúsculo
                    if (id.Contains("email", StringComparison.OrdinalIgnoreCase))
                    {
                        prop.SetValue(entidade, FormatHelper.ConverteParaMinusculo(rawValue));
                    }
                    // 🔹 Preço/Valor → decimal
                    else if (id.Contains("preco", StringComparison.OrdinalIgnoreCase) ||
                             id.Contains("valor", StringComparison.OrdinalIgnoreCase))
                    {
                        var normalizado = FormatHelper.SemFormatacao(rawValue, monetario: true);

                        if (decimal.TryParse(normalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out var dec))
                        {
                            prop.SetValue(entidade, dec);

                        }
                        else
                        {
                            if (Nullable.GetUnderlyingType(prop.PropertyType) != null)
                                prop.SetValue(entidade, null);
                            else
                                prop.SetValue(entidade, 0m);
                        }
                    }
                    // 🔹 Demais campos → apenas números
                    else
                    {
                        prop.SetValue(entidade, FormatHelper.SemFormatacao(rawValue));
                    }
                }
            }
        }
    }
}