using System.ComponentModel.DataAnnotations;

namespace Ordem_Servicos_Web.ViewModels
{
    public class UsuarioCreateViewModel
    {
        [Required(ErrorMessage = "Nome do Usuário é obrigatório")]
        [StringLength(150)]
        public string NomeUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "Login é obrigatório")]
        [StringLength(50)]
        public string Login { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha é obrigatória")]
        [StringLength(50)]
        [DataType(DataType.Password)]
        public string Senha { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirmação da senha é obrigatória")]
        [StringLength(50)]
        [DataType(DataType.Password)]
        [Compare("Senha", ErrorMessage = "As senhas não conferem")]
        public string ConfirmaSenha { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "E-mail inválido")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(8, ErrorMessage = "CEP deve ter 8 dígitos")]
        public string Cep { get; set; } = string.Empty;

        [StringLength(100)]
        public string Endereco { get; set; } = string.Empty;

        [StringLength(10)]
        public string Numero { get; set; } = string.Empty;

        [StringLength(50)]
        public string Bairro { get; set; } = string.Empty;

        [StringLength(100)]
        public string Municipio { get; set; } = string.Empty;

        [StringLength(2, ErrorMessage = "UF deve ter 2 caracteres")]
        public string Uf { get; set; } = string.Empty;

        [StringLength(11, ErrorMessage = "Telefone deve ter 10 ou 11 dígitos")]
        public string Fone1 { get; set; } = string.Empty;

        [StringLength(11, ErrorMessage = "Telefone deve ter 10 ou 11 dígitos")]
        public string Fone2 { get; set; } = string.Empty;

        [Required(ErrorMessage = "Data de cadastro é obrigatória")]
        public DateTime DataCadastro { get; set; }
        public byte[]? Imagem { get; set; }
    }
}
