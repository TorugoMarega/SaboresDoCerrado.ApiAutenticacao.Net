using System.ComponentModel.DataAnnotations;

namespace GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Usuario
{
    public class UpdateUserRequestDTO
    {
        [Required(ErrorMessage = "O nome completo é obrigatório.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O formato do e-mail é inválido.")]
        public string Email { get; set; }
    }
}
