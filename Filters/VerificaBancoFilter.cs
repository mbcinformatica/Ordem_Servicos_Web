using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Ordem_Servicos_Web.Data;

namespace Ordem_Servicos_Web.Filters
{
    public class VerificaBancoFilter(MeuDbContext context, ILogger<VerificaBancoFilter> logger) : IActionFilter
    {
        private readonly MeuDbContext _context = context;
        private readonly ILogger<VerificaBancoFilter> _logger = logger;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
                // Se já estamos na action ErroBanco, não aplica o filtro
                var actionName = context.ActionDescriptor.RouteValues["action"];
                var controllerName = context.ActionDescriptor.RouteValues["controller"];

                if (controllerName == "Home" && actionName == "ErroBanco")
                {
                    return;
                }
                if (!_context.Database.CanConnect())
                {
                    _logger.LogError("Banco de Dados Indisponível em {DataHora}", DateTime.Now);

                    // Usa TempData para enviar mensagem ao modal
                    context.HttpContext.Items["Mensagem"] = "Banco de Dados Indisponível.";
                    context.HttpContext.Items["MensagemTipo"] = "erro";

                    context.Result = new RedirectToActionResult("ErroBanco", "Home", null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Erro ao Tentar Conectar ao Banco em {DataHora}", DateTime.Now);

                context.HttpContext.Items["Mensagem"] = $"Erro ao conectar: {ex.Message}";
                context.HttpContext.Items["MensagemTipo"] = "erro";

                context.Result = new RedirectToActionResult("ErroBanco", "Home", null);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Nada a fazer depois da execução
        }
    }
}