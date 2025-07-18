using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Usuario;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.response;

namespace GoiabadaAtomica.ApiAutenticacao.Net.Service
{
    public interface IAuthService
    {
        Task<UsuarioDTO> ResgistrarAsync(RegistroRequestDTO registroRequestDTO);
        Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO);
        Task<bool> UpdateSenhaUsuarioPorIdAsync(int Id, UsuarioUpdateSenhaRequestDTO usuarioUpdateSenhaRequestDTO);
    }
}
