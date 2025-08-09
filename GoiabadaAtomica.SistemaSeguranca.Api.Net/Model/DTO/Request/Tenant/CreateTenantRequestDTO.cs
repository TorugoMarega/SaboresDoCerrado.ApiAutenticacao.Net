using System.ComponentModel.DataAnnotations;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Request.Tenant
{
    public class CreateTenantRequestDTO
    {
        [Required(ErrorMessage = "O nome da empresa é obrigatório.")]
        [MaxLength(150, ErrorMessage = "O nome da empresa deve ter no máximo 150 caracteres.")]
        public string Name { get; set; }

        [MaxLength(255, ErrorMessage = "O nome do contato deve ter no máximo 255 caracteres.")]
        public string? ContactPerson { get; set; }

        [EmailAddress(ErrorMessage = "O e-mail de contato é inválido.")]
        [MaxLength(150, ErrorMessage = "O e-mail de contato deve ter no máximo 150 caracteres.")]
        public string? ContactEmail { get; set; }

        [MaxLength(20, ErrorMessage = "O telefone de contato deve ter no máximo 20 caracteres.")]
        public string? ContactPhone { get; set; }
    }
}
