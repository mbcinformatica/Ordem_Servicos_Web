namespace Ordem_Servicos_Web.Models
{
    public class ErrorViewModel
    {
        // Propriedades para exibir informações de erro
        public string? RequestId { get; set; }

        // Propriedade para indicar se o RequestId deve ser exibido
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        // Propriedades para exibir detalhes do erro
        public string? MensagemErro { get; set; }

        // Propriedade para exibir a pilha de chamadas do erro
        public string? StackTrace { get; set; }
    }
}