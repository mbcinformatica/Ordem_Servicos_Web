using Ordem_Servicos_Web.Models;

namespace Ordem_Servicos_Web.Services.Interfaces
{
    public interface ICnpjService<T>
    {
        /// <summary>
        /// Consulta os dados de um CNPJ em uma API externa
        /// </summary>
        /// <param name="cnpj">Número do CNPJ sem formatação</param>
        /// <returns>Objeto Cliente preenchido ou null se não encontrado</returns>
        Task<T?> ConsultarCnpjAsync(string cnpj);
    }
}