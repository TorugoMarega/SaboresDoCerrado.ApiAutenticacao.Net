using System.ComponentModel.DataAnnotations;

namespace GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Perfil
{
    public class UpdateRolelRequestDTO
    {
        [Required(ErrorMessage = "O nome do perfil é obrigatório.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "A descrição do perfil é obrigatória.")]
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
