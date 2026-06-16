namespace Ordem_Servicos_Web.ViewModels
{
    public class UsuarioViewModel
    {
        public int IdUsuario { get; set; }

        public string NomeUsuario { get; set; } = string.Empty;

        public string Login { get; set; } = string.Empty;

        public string? Senha { get; set; }

        public string? ConfirmaSenha { get; set; }

        public string Cep { get; set; } = string.Empty;

        public string Endereco { get; set; } = string.Empty;

        public string? Numero { get; set; }

        public string Bairro { get; set; } = string.Empty;

        public string Municipio { get; set; } = string.Empty;

        public string Uf { get; set; } = string.Empty;

        public string? FoneFixo { get; set; }

        public string? FoneCelular { get; set; }

        public string? Email { get; set; }

        public DateTime DataCadastro { get; set; }

        public byte[]? Imagem { get; set; }
    }
}