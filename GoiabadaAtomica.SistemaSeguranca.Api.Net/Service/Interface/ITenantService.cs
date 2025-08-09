using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Request.Tenant;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response.Tenant;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Interface
{
    public interface ITenantService
    {
        Task<TenantDTO> CreateTenantAsync(CreateTenantRequestDTO createTenantRequestDTO);
        Task<TenantDTO?> UpdateTenantAsync(int id, UpdateTenantRequestDTO updateTenantRequestDTO);
        Task<bool?> DeactivateActivateTenantAsync(int id, bool newStatus);
        Task<IEnumerable<TenantDTO>> GetAllTenantAsync();
        Task<TenantDTO?> GetTenantByIdAsync(int id);
        Task<bool> ExistsTenantByIdAsync(int clientId);
    }
}
