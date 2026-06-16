using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
<<<<<<< HEAD
=======
using Microsoft.Extensions.Logging;
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
using Ordem_Servicos_Web.Data;

namespace Ordem_Servicos_Web.Filters
{
<<<<<<< HEAD
    public class VerificaBancoFilter(MeuDbContext context, ILogger<VerificaBancoFilter> logger) : IActionFilter
    {
        private readonly MeuDbContext _context = context;
        private readonly ILogger<VerificaBancoFilter> _logger = logger;
=======
    public class VerificaBancoFilter : IActionFilter
    {
        private readonly MeuDbContext _context;
        private readonly ILogger<VerificaBancoFilter> _logger;

        public VerificaBancoFilter(MeuDbContext context, ILogger<VerificaBancoFilter> logger)
        {
            _context = context;
            _logger = logger;
        }
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7

        public void OnActionExecuting(ActionExecutingContext context)
        {
            try
            {
<<<<<<< HEAD
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

=======
                if (!_context.Database.CanConnect())
                {
                    _logger.LogError("Banco de Dados Indisponível em {DataHora}", DateTime.Now);
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
                    context.Result = new RedirectToActionResult("ErroBanco", "Home", null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Erro ao Tentar Conectar ao Banco em {DataHora}", DateTime.Now);
<<<<<<< HEAD

                context.HttpContext.Items["Mensagem"] = $"Erro ao conectar: {ex.Message}";
                context.HttpContext.Items["MensagemTipo"] = "erro";

=======
>>>>>>> 2ddd7e7f7bdc32e421f23a847a01c8ab48c6f1d7
                context.Result = new RedirectToActionResult("ErroBanco", "Home", null);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Nada a fazer depois da execução
        }
    }
}