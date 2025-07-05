using System.ComponentModel.DataAnnotations;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO.request
{
    public class RegistroRequestDTO
    {
        [RegularExpression(@"^\S*$", ErrorMessage = "O nome de usuário não pode conter espaços.")]
        [Required(ErrorMessage = "O identificador de usuário é obrigatório.")]
        public string NomeUsuario { get; set; }

        [Required(ErrorMessage = "O nome de usuário é obrigatório.")]
        public string NomeCompleto { get; set; }
        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O formato do e-mail é inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
        public string Senha { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "O usuário deve ter no mínimo um perfil.")]
        public List<int> PerfilIds { get; set; }
    }
}
