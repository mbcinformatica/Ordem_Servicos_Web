using System.ComponentModel.DataAnnotations;

namespace Ordem_Servicos_Web.ViewModels
{
    public class UsuarioEditViewModel
    {
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "Nome do Usuário é obrigatório")]
        [StringLength(150)]
        public string NomeUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "Login é obrigatório")]
        [StringLength(50)]
        public string Login { get; set; } = string.Empty;

        [StringLength(50)]
        [DataType(DataType.Password)]
        public string? Senha { get; set; }

        [StringLength(50)]
        [DataType(DataType.Password)]
        [Compare("Senha", ErrorMessage = "As senhas não conferem")]
        public string? ConfirmaSenha { get; set; }

        [StringLength(8)]
        public string Cep { get; set; } = string.Empty;

        [StringLength(100)]
        public string Endereco { get; set; } = string.Empty;

        [StringLength(10)]
        public string Numero { get; set; } = string.Empty;

        [StringLength(50)]
        public string Bairro { get; set; } = string.Empty;

        [StringLength(100)]
        public string Municipio { get; set; } = string.Empty;

        [StringLength(2)]
        public string Uf { get; set; } = string.Empty;

        [StringLength(11)]
        public string Fone1 { get; set; } = string.Empty;

        [StringLength(11)]
        public string Fone2 { get; set; } = string.Empty;

        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data de cadastro é obrigatória")]
        public DateTime DataCadastro { get; set; }

        public byte[]? Imagem { get; set; }
    }
}
