using GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO;
using GoiabadaAtomica.ApiAutenticacao.Net.Model.entity;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Interface
{
    public interface IUserRepository
    {
        Task<User> RegisterUserAsync(User user);
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<UserDTO?> GetUserByIdAsync(int id);
        Task<User?> GetUserEntityByIdAsync(int id);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> DeactivateActivateUserAsync(int id, bool status);
        Task<bool> EmailExistsInAnotherUserAsync(int id, string email);
        Task UpdateUserAsync(User user);
    }
}