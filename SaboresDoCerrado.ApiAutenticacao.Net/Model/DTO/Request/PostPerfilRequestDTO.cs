using System.ComponentModel.DataAnnotations;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO.Request
{
    public class PostPerfilRequestDTO
    {
        [Required(ErrorMessage = "O nome do perfil é obrigatório.")]
        public required string Nome { get; set; }
        [Required(ErrorMessage = "A descrição do perfil é obrigatória.")]
        public required string Descricao { get; set; }
    }
}
