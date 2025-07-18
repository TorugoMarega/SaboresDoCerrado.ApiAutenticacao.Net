using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.Request.Usuario;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO.response;

namespace GoiabadaAtomica.ApiAutenticacao.Net.Service
{
    public interface IAuthService
    {
        Task<UserDTO> RegisterAsync(RegistrationRequestDTO RegistrationRequestDTO);
        Task<LoginResponseDTO?> LoginAsync(LoginRequestDTO loginRequestDTO);
        Task<bool> UpdateUserPasswordByIdAsync(int Id, UpdateUserPasswordRequestDTO updateUserPasswordRequestDTO);
    }
}