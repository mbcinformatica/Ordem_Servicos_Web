namespace Ordem_Servicos_Web.Services.Interfaces;

public interface IImageService
{
    object ProcessarImagem(byte[] imagem, string nome, string codigo);
}