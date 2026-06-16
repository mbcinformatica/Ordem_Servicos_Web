using System.Globalization;
using System.Text.RegularExpressions;

namespace Ordem_Servicos_Web.Helpers
{
    public static partial class FormatHelper
    {
        // Regex compilada em tempo de execução para remover caracteres não numéricos
        private static readonly Regex _nonDigitRegex = new(@"\D", RegexOptions.Compiled);
        private static Regex NonDigitRegex() => _nonDigitRegex;
        public static string FormatCpfCnpj(string valor, string tipoPessoa)
        {
            valor = SomenteNumeros(valor);
            if (string.IsNullOrEmpty(valor)) return "";

            if (tipoPessoa == "JURÍDICA" && valor.Length == 14)
                return $"{valor[..2]}.{valor.Substring(2, 3)}.{valor.Substring(5, 3)}/{valor.Substring(8, 4)}-{valor.Substring(12, 2)}";

            if (tipoPessoa == "FÍSICA" && valor.Length == 11)
                return $"{valor[..3]}.{valor.Substring(3, 3)}.{valor.Substring(6, 3)}-{valor.Substring(9, 2)}";

            return valor;
        }

        public static string FormatTelefone(string valor)
        {
            valor = SomenteNumeros(valor);
            if (string.IsNullOrEmpty(valor)) return "";

            if (valor.Length == 11)
                return $"({valor[..2]}) {valor.Substring(2, 5)}-{valor.Substring(7, 4)}";

            if (valor.Length == 10)
                return $"({valor[..2]}) {valor.Substring(2, 4)}-{valor.Substring(6, 4)}";

            return valor;
        }

        public static string FormatCep(string valor)
        {
            valor = SomenteNumeros(valor);
            if (string.IsNullOrEmpty(valor)) return "";

            if (valor.Length == 8)
                return $"{valor[..5]}-{valor.Substring(5, 3)}";

            return valor;
        }

        public static string ConverteParaMinusculo(string? valor)
        {
            return string.IsNullOrEmpty(valor) ? "" : valor.ToLower();
        }

        public static string SomenteNumeros(string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return "";
            return NonDigitRegex().Replace(valor, "");
        }

        public static string SemFormatacao(string? valor)
        {
            return SomenteNumeros(valor);
        }

        public static string FormatValor(decimal valor)
        {
            return valor.ToString("C", new CultureInfo("pt-BR"));
        }

        public static string FormatQuantidade(int quantidade)
        {
            return quantidade.ToString("N0", new CultureInfo("pt-BR"));
        }
    }
}
