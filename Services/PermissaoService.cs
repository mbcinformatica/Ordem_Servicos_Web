using Ordem_Servicos_Web.Data;
using Ordem_Servicos_Web.Models;

namespace Ordem_Servicos_Web.Services
{
    public class PermissaoService(MeuDbContext context)
    {
        private readonly MeuDbContext _context = context;

        private Permissao? ObterPermissaoPorNome(int idUsuario, string nomeMenu, string nomeItemMenu)
        {
            // Normaliza para maiúsculo
            nomeMenu = nomeMenu.ToUpper();
            nomeItemMenu = nomeItemMenu.ToUpper();

            var idMenu = _context.Menus
                .Where(m => m.Descricao.ToUpper() == nomeMenu)
                .Select(m => m.IdMenu)
                .FirstOrDefault();

            var idItensMenu = _context.ItensMenus
                .Where(i => i.Descricao.ToUpper() == nomeItemMenu && i.IdMenu == idMenu)
                .Select(i => i.IdItensMenu)
                .FirstOrDefault();

            if (idMenu == 0 || idItensMenu == 0)
                return null;

            return _context.Permissoes
                .FirstOrDefault(p => p.IdUsuario == idUsuario
                                  && p.IdMenu == idMenu
                                  && p.IdItensMenu == idItensMenu);
        }

        public bool PodeExecutar(int idUsuario, string nomeMenu, string nomeItemMenu)
            => ObterPermissaoPorNome(idUsuario, nomeMenu, nomeItemMenu)?.Executar ?? false;

        public bool PodeCriar(int idUsuario, string nomeMenu, string nomeItemMenu)
            => ObterPermissaoPorNome(idUsuario, nomeMenu, nomeItemMenu)?.Criar ?? false;

        public bool PodeAlterar(int idUsuario, string nomeMenu, string nomeItemMenu)
            => ObterPermissaoPorNome(idUsuario, nomeMenu, nomeItemMenu)?.Alterar ?? false;

        public bool PodeExcluir(int idUsuario, string nomeMenu, string nomeItemMenu)
            => ObterPermissaoPorNome(idUsuario, nomeMenu, nomeItemMenu)?.Excluir ?? false;
    }
}