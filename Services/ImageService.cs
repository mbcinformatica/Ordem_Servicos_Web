using Ordem_Servicos_Web.Services.Interfaces;

namespace Ordem_Servicos_Web.Services
{
    public class ImageService : IImageService
    {
        public object ProcessarImagem(byte[] imagem, string nome, string codigo)
        {
            if (imagem == null || imagem.Length == 0)
                return new { exists = false };

            string contentType = "image/jpeg"; // padrão
            if (imagem.Length > 4)
            {
                if (imagem[0] == 0x89 && imagem[1] == 0x50)
                    contentType = "image/png";
                else if (imagem[0] == 0x47 && imagem[1] == 0x49)
                    contentType = "image/gif";
            }

            string imagemBase64 = Convert.ToBase64String(imagem);

            return new
            {
                exists = true,
                nome,
                codigo,
                imagemBase64,
                contentType
            };
        }
    }
}
