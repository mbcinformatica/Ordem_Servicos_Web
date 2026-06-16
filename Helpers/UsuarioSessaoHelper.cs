using Microsoft.AspNetCore.Http;

namespace Ordem_Servicos_Web.Helpers
{
    public static class UsuarioSessaoHelper
    {
        public static int ObterUsuarioLogado(HttpContext httpContext)
        {
            return int.Parse(httpContext.Session.GetString("IdUsuario") ?? "0");
        }
    }
}