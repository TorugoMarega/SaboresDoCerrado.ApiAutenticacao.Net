using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO.Request.Usuario;
using SaboresDoCerrado.ApiAutenticacao.Net.Model.DTO.response;

namespace SaboresDoCerrado.ApiAutenticacao.Net.Service
{
    public interface IAuthService
    {
        Task<UsuarioDTO> ResgistrarAsync(RegistroRequestDTO registroRequestDTO);
        Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO);
    }
}
