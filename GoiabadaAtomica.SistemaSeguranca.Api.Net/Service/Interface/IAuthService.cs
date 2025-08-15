using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Usuario;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Request.Auth;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response.Auth;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Interface
{
    public interface IAuthService
    {
        Task<UserDTO> RegisterAsync(int tenantId, RegistrationRequestDTO registrationRequestDTO);
        Task<LoginResponseDTO?> LoginAsync(int tenantId, LoginRequestDTO loginRequestDTO);
        Task<bool> UpdateUserPasswordByIdAsync(int tenantId, int UserId, UpdateUserPasswordRequestDTO updateUserPasswordRequestDTO);
    }
}