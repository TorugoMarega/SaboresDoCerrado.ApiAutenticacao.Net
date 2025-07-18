using System.ComponentModel.DataAnnotations;

namespace GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Perfil
{
    public class PostPerfilRequestDTO
    {
        [Required(ErrorMessage = "O nome do perfil é obrigatório.")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "A descrição do perfil é obrigatória.")]
        public string Descricao { get; set; }
    }
}
