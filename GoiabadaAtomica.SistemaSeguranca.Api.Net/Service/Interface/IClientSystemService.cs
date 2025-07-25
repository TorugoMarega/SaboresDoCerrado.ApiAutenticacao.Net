using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Request.ClientSystem;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response.ClientSystem;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Interface
{
    public interface IClientSystemService
    {
        Task<CreateClientSystemResponseDTO> CreateClientSystemAsync(CreateClientSystemRequestDTO createClientSystemRequestDTO);
        Task<ClientSystemDTO?> UpdateClientSystemAsync(int id, UpdateClientSystemRequestDTO updateClientSystemRequestDTO);
        Task<bool?> DeactivateActivateClientSystemAsync(int id, bool newStatus);
        Task<IEnumerable<ClientSystemDTO>> GetAllClientSystemAsync();
        Task<ClientSystemDTO?> GetClientSystemByIdAsync(int id);
    }
}
