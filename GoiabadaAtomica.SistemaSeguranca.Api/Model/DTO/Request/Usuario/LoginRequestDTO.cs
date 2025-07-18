using System.ComponentModel.DataAnnotations;

namespace GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Usuario
{
    public class LoginRequestDTO
    {
        [RegularExpression(@"^\S*$", ErrorMessage = "O nome de usuário não pode conter espaços.")]
        [Required(ErrorMessage = "O identificador de usuário é obrigatório.")]
        public string NomeUsuario { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        public string Senha { get; set; }
    }
}
