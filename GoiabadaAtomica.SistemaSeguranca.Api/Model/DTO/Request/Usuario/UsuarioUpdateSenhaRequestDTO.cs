using System.ComponentModel.DataAnnotations;

namespace GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Usuario
{
    public class UsuarioUpdateSenhaRequestDTO
    {
        [Required(ErrorMessage = "Obrigatório informar a senha atual.")]
        public string SenhaAtual { get; set; }

        [Required(ErrorMessage = "Obrigatório informar a nova senha.")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
        public string NovaSenha { get; set; }

        [Compare("NovaSenha", ErrorMessage = "'ConfimacaoSenha' e 'NovaSenha' não combinam.")]
        [Required(ErrorMessage = "Obrigatório informar a confirmação da nova senha.")]
        public string ConfimacaoSenha { get; set; }
    }
}
