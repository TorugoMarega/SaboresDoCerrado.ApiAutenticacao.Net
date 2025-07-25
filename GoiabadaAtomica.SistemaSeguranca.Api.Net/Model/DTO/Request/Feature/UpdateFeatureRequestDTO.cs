using System.ComponentModel.DataAnnotations;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Request.Feature
{
    public interface UpdateFeatureRequestDTO
    {
        [Required(ErrorMessage = "O nome da funcionalidade é obrigatório.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "A descrição da funcionalidade é obrigatória.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "O status da funcionalidade é obrigatório.")]
        public bool IsActive { get; set; }
    }
}
