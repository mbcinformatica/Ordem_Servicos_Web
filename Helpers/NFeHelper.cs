using System.IO.Compression;
using System.Text;

namespace Ordem_Servicos_Web.Helpers
{
    public class NFeHelper
    {
        public static string ExtrairXmlDeDocZip(string conteudoBase64)
        {
            // Converter de Base64 para bytes
            byte[] dadosCompactados = Convert.FromBase64String(conteudoBase64);

            // Descompactar GZip
            using var memoria = new MemoryStream(dadosCompactados);
            using var gzip = new GZipStream(memoria, CompressionMode.Decompress);
            using var destino = new MemoryStream();
            gzip.CopyTo(destino);
            byte[] dadosXml = destino.ToArray();

            // Converter para string UTF-8
            return Encoding.UTF8.GetString(dadosXml);
        }
    }
}