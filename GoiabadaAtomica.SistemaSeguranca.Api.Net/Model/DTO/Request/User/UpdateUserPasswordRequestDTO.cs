using System.ComponentModel.DataAnnotations;

namespace GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Usuario
{
    public class UpdateUserPasswordRequestDTO
    {
        [Required(ErrorMessage = "Obrigatório informar a senha atual.")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Obrigatório informar a nova senha.")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "'ConfirmPassword' e 'NewPassword' não combinam.")]
        [Required(ErrorMessage = "Obrigatório informar a confirmação da nova senha.")]
        public string ConfirmPassword { get; set; }
    }
}
