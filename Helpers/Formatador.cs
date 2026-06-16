using System.Text.RegularExpressions;

namespace Ordem_Servicos_Web.Helpers
{
    public static class Formatador
    {
        public static string FormatCpfCnpj(string valor, string tipoPessoa)
        {
            if (string.IsNullOrEmpty(valor)) return "";

            valor = Regex.Replace(valor, @"\D", "");
            if (tipoPessoa == "JURÍDICA" && valor.Length == 14)
                return Convert.ToUInt64(valor).ToString(@"00\.000\.000\/0000\-00");
            else if (tipoPessoa == "FÍSICA" && valor.Length == 11)
                return Convert.ToUInt64(valor).ToString(@"000\.000\.000\-00");

            return valor;
        }
        public static string FormatTelefone(string valor)
        {
            if (string.IsNullOrEmpty(valor)) return "";

            valor = Regex.Replace(valor, @"\D", "");
            if (valor.Length == 11)
                return Convert.ToUInt64(valor).ToString(@"\(00\) 00000\-0000");
            else if (valor.Length == 10)
                return Convert.ToUInt64(valor).ToString(@"\(00\) 0000\-0000");

            return valor;
        }
        public static string FormatCep(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return "";

            valor = Regex.Replace(valor, @"\D", "");
            if (valor.Length == 8)
            {
                return Regex.Replace(valor, @"(\d{5})(\d{3})", "$1-$2");
            }
            return valor;
        }
        public static string ConverteParaMinusculo(string valor)
        {
            return string.IsNullOrEmpty(valor) ? "" : valor.ToLower();
        }
    }
}