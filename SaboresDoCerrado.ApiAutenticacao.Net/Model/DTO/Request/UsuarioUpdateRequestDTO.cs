using System.ComponentModel.DataAnnotations;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO.Request
{
    public class UsuarioUpdateRequestDTO
    {
        [Required(ErrorMessage = "O nome completo é obrigatório.")]
        public string NomeCompleto { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O formato do e-mail é inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O status do usuário é obrigatório.")]
        public bool IsAtivo { get; set; }
    }
}
