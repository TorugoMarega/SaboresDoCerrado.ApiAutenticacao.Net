using System.ComponentModel.DataAnnotations;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO.Request
{
    public class UpdatePerfilRequestDTO
    {
        [Required(ErrorMessage = "O nome do perfil é obrigatório.")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "A descrição do perfil é obrigatória.")]
        public string Descricao { get; set; }
        public bool Status { get; set; }
    }
}
