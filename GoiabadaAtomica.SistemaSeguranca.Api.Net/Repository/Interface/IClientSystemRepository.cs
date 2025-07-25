using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response.ClientSystem;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.Entity;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Interface
{
    public interface IClientSystemRepository
    {
        Task<ClientSystemEntity?> GetClientSystemEntityByIdAsync(int id);
        Task<ClientSystemDTO?> GetClientSystemDTOByIdAsync(int id);
        Task<IEnumerable<ClientSystemDTO>> GetAllClientSystemByAsync();
        Task<CreateClientSystemResponseDTO> CreateClientSystemAsync(ClientSystemEntity clientSystem);
        Task<int> UpdateClientSystemAsync(ClientSystemEntity clientSystem);
        Task<bool> ExistsClientSystemById(int id);
        Task<bool> ExistsClientSystemByNameAsync(string name);
        Task<bool> ExistsClientSystemByClientIdAsync(string clientId);
    }
}
