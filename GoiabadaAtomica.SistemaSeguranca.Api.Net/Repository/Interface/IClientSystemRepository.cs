using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response.ClientSystem;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.Entity;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Interface
{
    public interface IClientSystemRepository
    {
        Task<ClientSystemEntity?> GetClientSystemEntityByIdAsync(int tenantId, int id);
        Task<ClientSystemDTO?> GetClientSystemDTOByIdAsync(int tenantId, int clientSystemId);
        Task<IEnumerable<ClientSystemDTO>> GetAllClientSystemByAsync(int tenantId);
        Task<CreateClientSystemResponseDTO> CreateClientSystemAsync(ClientSystemEntity clientSystem);
        Task<int> UpdateClientSystemAsync(ClientSystemEntity clientSystem);
        Task<bool> ExistsClientSystemById(int tenantId, int clientSystemId);
        Task<bool> ExistsClientSystemByNameAsync(int tenantId, string clientSystemName);
        Task<bool> ExistsClientSystemByClientIdAsync(int tenantId, string clientId);
    }
}
