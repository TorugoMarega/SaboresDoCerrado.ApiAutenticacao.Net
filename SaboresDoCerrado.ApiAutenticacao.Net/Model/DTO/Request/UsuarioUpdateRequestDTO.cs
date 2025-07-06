using System.ComponentModel.DataAnnotations;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO.Request
{
    public class UsuarioUpdateRequestDTO
    {
        [Required(ErrorMessage = "O nome completo é obrigatório.")]
        public required string NomeCompleto { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O formato do e-mail é inválido.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "O status do usuário é obrigatório.")]
        public required bool IsAtivo { get; set; }

        [Required]
        public required List<int> PerfilIds { get; set; }
    }
}
