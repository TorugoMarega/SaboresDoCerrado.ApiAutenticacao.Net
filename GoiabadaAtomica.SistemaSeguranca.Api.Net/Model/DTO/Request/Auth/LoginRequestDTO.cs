using System.ComponentModel.DataAnnotations;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Request.Auth
{
    public class LoginRequestDTO
    {
        [RegularExpression(@"^\S*$", ErrorMessage = "O nome de usuário não pode conter espaços.")]
        [Required(ErrorMessage = "O identificador de usuário é obrigatório.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        public string Password { get; set; }
    }
}
