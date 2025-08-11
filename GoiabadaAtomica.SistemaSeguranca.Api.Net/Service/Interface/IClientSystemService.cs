using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Request.ClientSystem;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response.ClientSystem;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Interface
{
    public interface IClientSystemService
    {
        Task<CreateClientSystemResponseDTO> CreateClientSystemAsync(int tenantId, CreateClientSystemRequestDTO createClientSystemRequestDTO);
        Task<ClientSystemDTO?> UpdateClientSystemAsync(int tenantId, int clientSystemId, UpdateClientSystemRequestDTO updateClientSystemRequestDTO);
        Task<bool?> DeactivateActivateClientSystemAsync(int tenantId, int clientSystemId, bool newStatus);
        Task<IEnumerable<ClientSystemDTO>> GetAllClientSystemAsync(int tenantId);
        Task<ClientSystemDTO?> GetClientSystemByIdAsync(int tenantId, int clientSystemId);
        Task<bool> ExistsClientSystemByIdAsync(int tenantId, int clientSystemId);
    }
}
