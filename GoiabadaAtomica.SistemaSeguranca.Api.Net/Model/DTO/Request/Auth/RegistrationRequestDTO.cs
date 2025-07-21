using System.ComponentModel.DataAnnotations;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Request.Auth
{
    public class RegistrationRequestDTO
    {
        [RegularExpression(@"^\S*$", ErrorMessage = "O nome de usuário não pode conter espaços.")]
        [Required(ErrorMessage = "O identificador de usuário é obrigatório.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O formato do e-mail é inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
        public string Password { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "O usuário deve ter no mínimo um perfil.")]
        public List<int> RoleIds { get; set; }
    }
}
