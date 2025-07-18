using System.ComponentModel.DataAnnotations;

namespace GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Usuario
{
    public class UsuarioUpdateRequestDTO
    {
        [Required(ErrorMessage = "O nome completo é obrigatório.")]
        public string NomeCompleto { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O formato do e-mail é inválido.")]
        public string Email { get; set; }
    }
}
