using Ordem_Servicos_Web.Helpers;
using Ordem_Servicos_Web.Models;
using Ordem_Servicos_Web.Services.Interfaces;
using System.Text.Json;

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
                    CpfCnpj = FormatHelper.SemFormatacao(cnpj) ?? string.Empty,
                    NomeRazaoSocial = dados.GetProperty("nome").GetString() ?? "",
                    Endereco = dados.GetProperty("logradouro").GetString(),
                    Numero = dados.GetProperty("numero").GetString(),
                    Bairro = dados.GetProperty("bairro").GetString(),
                    Municipio = dados.GetProperty("municipio").GetString(),
                    Uf = dados.GetProperty("uf").GetString(),
                    Cep = FormatHelper.SemFormatacao(cep) ?? string.Empty,
                    FoneFixo = FormatHelper.SemFormatacao(foneFixo) ?? string.Empty,
                    FoneCelular = FormatHelper.SemFormatacao(foneCelular) ?? string.Empty,
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
    }
}