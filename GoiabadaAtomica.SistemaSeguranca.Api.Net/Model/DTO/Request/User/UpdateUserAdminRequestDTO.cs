using System.ComponentModel.DataAnnotations;

namespace GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Usuario
{
    public class UpdateUserAdminRequestDTO
    {
        [Required(ErrorMessage = "O nome completo é obrigatório.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O formato do e-mail é inválido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O status do usuário é obrigatório.")]
        public bool IsActive { get; set; }

        [Required]
        public List<int> RoleIds { get; set; }
    }
}
