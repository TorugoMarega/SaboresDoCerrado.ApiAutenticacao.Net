using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response.Tenant;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.Entity;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Interface
{
    public interface ITenantRepository
    {
        Task<TenantEntity?> GetTenantEntityByIdAsync(int id);
        Task<TenantDTO?> GetTenantDTOByIdAsync(int id);
        Task<IEnumerable<TenantDTO>> GetAllTenantByAsync();
        Task<TenantDTO> CreateTenantAsync(TenantEntity Tenant);
        Task<int> UpdateTenantAsync(TenantEntity Tenant);
        Task<bool> ExistsTenantById(int id);
        Task<bool> ExistsTenantByNameAsync(string name);
        Task<bool> ExistsTenantByNameIdExcludeIdAsync(string name, int id);
    }
}
