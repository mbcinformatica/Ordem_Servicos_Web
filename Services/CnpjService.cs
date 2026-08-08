using Ordem_Servicos_Web.Helpers;
using Ordem_Servicos_Web.Models;
using Ordem_Servicos_Web.Services.Interfaces;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text.Json;
using System.Xml;

namespace Ordem_Servicos_Web.Services
{
    public class CnpjService<T> : ICnpjService<T> where T : PessoaFisicaJuridica, new()
    {
        private readonly HttpClient _http;
        private readonly ILogger<CnpjService<T>> _logger;

        public CnpjService(HttpClient http, ILogger<CnpjService<T>> logger)
        {
            _http = http;
            _logger = logger;
            _http.Timeout = TimeSpan.FromSeconds(20);
            _http.DefaultRequestHeaders.Add("Accept", "application/json");
        }

        public async Task<T?> ConsultarCnpjAsync(string cnpj)
        {
            try
            {
                var response = await _http.GetAsync($"https://www.receitaws.com.br/v1/cnpj/{cnpj}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Erro ao consultar CNPJ {cnpj}: {response.StatusCode}");
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var dados = JsonSerializer.Deserialize<JsonElement>(json);

                if (dados.TryGetProperty("status", out var status) && status.GetString() == "ERROR")
                {
                    _logger.LogWarning($"CNPJ {cnpj} não encontrado: {dados.GetProperty("message").GetString()}");
                    return null;
                }

                // 🔹 Mapeamento genérico para PessoaJuridica
                var telefoneApi = dados.TryGetProperty("telefone", out var telProp) ? telProp.GetString() : null;
                var partesTelefone = telefoneApi?.Split('/') ?? Array.Empty<string>();

                var foneCelular = partesTelefone.Length > 0 ? partesTelefone[0].Trim() : null;
                var foneFixo = partesTelefone.Length > 1 ? partesTelefone[1].Trim() : null;

                var cep = dados.TryGetProperty("cep", out var cepProp) ? cepProp.GetString() : null;
                cep = cep != null ? new string(cep.Where(char.IsDigit).ToArray()) : null;

                return new T
                {
                    TipoPessoa = "JURÍDICA",
                    CpfCnpj = FormatHelper.SomenteNumeros(cnpj) ?? string.Empty,
                    NomeRazaoSocial = dados.GetProperty("nome").GetString() ?? "",
                    Endereco = dados.GetProperty("logradouro").GetString(),
                    Numero = dados.GetProperty("numero").GetString(),
                    Bairro = dados.GetProperty("bairro").GetString(),
                    Municipio = dados.GetProperty("municipio").GetString(),
                    Uf = dados.GetProperty("uf").GetString(),
                    Cep = FormatHelper.SomenteNumeros(cep) ?? string.Empty,
                    FoneFixo = FormatHelper.SomenteNumeros(foneFixo) ?? string.Empty,
                    FoneCelular = FormatHelper.SomenteNumeros(foneCelular) ?? string.Empty,
                    Email = dados.GetProperty("email").GetString(),
                    Contato = dados.GetProperty("email").GetString(),
                    DataCadastro = DateTime.Now
                } as T;
            }
            catch (TaskCanceledException)
            {
                _logger.LogError($"Timeout ao consultar CNPJ {cnpj}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao consultar CNPJ {cnpj}");
                return null;
            }
        }

        public void ConsultarNotas(string cnpj, DateTime inicio, DateTime fim)
        {
            var certificado = new X509Certificate2("caminho_do_certificado.pfx", "senha");

            string url = "https://nfe.sefazvirtual.rs.gov.br/ws/NFeDistribuicaoDFe/NFeDistribuicaoDFe.asmx";

            var binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
            binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;

            var endpoint = new EndpointAddress(url);

            var client = new NFeDistribuicaoDFeClient(binding, endpoint)
            {
                ClientCredentials = new System.ServiceModel.Description.ClientCredentials()
            };
            // usando o tipo do WCF explicitamente
            if (client.ClientCredentials is System.ServiceModel.Description.ClientCredentials creds)
            {
                creds.ClientCertificate.Certificate = certificado;
            }
            else
            {
                _logger.LogError("ClientCredentials do cliente WCF não é do tipo ClientCredentials.");
                throw new InvalidOperationException("ClientCredentials inválido.");
            }

            string? ultNSU = "0";
            bool continuar = true;

            while (continuar)
            {
                string xml = $@"
            <distDFeInt xmlns=""http://www.portalfiscal.inf.br/nfe"" versao=""1.01"">
                <tpAmb>1</tpAmb>
                <cUFAutor>42</cUFAutor> <!-- SC -->
                <CNPJ>{cnpj}</CNPJ>
                <distNSU>
                    <ultNSU>{ultNSU}</ultNSU>
                </distNSU>
            </distDFeInt>";

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                var respostaObj = client.nfeDistDFeInteresse(doc);

                // Normaliza para XmlDocument / XmlNode para poder usar GetElementsByTagName
                XmlDocument xmlResposta;
                if (respostaObj is XmlDocument xd)
                {
                    xmlResposta = xd;
                }
                else if (respostaObj is XmlNode xn)
                {
                    xmlResposta = new XmlDocument();
                    xmlResposta.LoadXml(xn.OuterXml);
                }
                else if (respostaObj is string s)
                {
                    xmlResposta = new XmlDocument();
                    xmlResposta.LoadXml(s);
                }
                else
                {
                    _logger.LogError("Tipo de resposta inesperado ao chamar nfeDistDFeInteresse: {Tipo}", respostaObj?.GetType());
                    throw new InvalidOperationException("Tipo de resposta inesperado.");
                }

                XmlNodeList documentos = xmlResposta.GetElementsByTagName("docZip");
                foreach (XmlNode docZip in documentos)
                {
                    string conteudo = docZip.InnerText;
                    // descompactar (Base64 + GZip) e ler o XML da NF-e
                }
                var maxNSU = xmlResposta.GetElementsByTagName("maxNSU")[0]?.InnerText;
                ultNSU = xmlResposta.GetElementsByTagName("ultNSU")[0]?.InnerText;

                if (ultNSU == maxNSU)
                    continuar = false;
            }
        }
    }
}