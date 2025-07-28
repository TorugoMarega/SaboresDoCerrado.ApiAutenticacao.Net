using System.ComponentModel.DataAnnotations;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Request.Feature
{
    public class CreateFeatureRequestDTO
    {
        [Required(ErrorMessage = "O nome da funcionalidade é obrigatório.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "A descrição da funcionalidade é obrigatória.")]
        public string Description { get; set; }
    }
}
