using GoiabadaAtomica.ApiAutenticacao.Net.Model.entity;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Interface
{
    public interface IUserRepository
    {
        Task<UserEntity> RegisterUserAsync(UserEntity user);
        Task<IEnumerable<UserDTO>> GetAllUsersAsync(int tenantId);
        Task<UserDTO?> GetUserByIdAsync(int tenantId, int userId);
        Task<UserEntity?> GetUserEntityByIdAsync(int tenantId, int userId);
        Task<UserEntity?> GetUserByUsernameAsync(int tenantId, string username);
        Task<bool> EmailExistsAsync(int tenantId, string email);
        Task<bool> UsernameExistsAsync(int tenantId, string username);
        Task<bool> DeactivateActivateUserAsync(int tenantId, int userId, bool status);
        Task<bool> EmailExistsInAnotherUserAsync(int tenantId, int userId, string email);
        Task UpdateUserAsync(UserEntity user);
        Task<UserEntity?> GetByUsernameWithTenantAsync(int tenantId, string username);
    }
}