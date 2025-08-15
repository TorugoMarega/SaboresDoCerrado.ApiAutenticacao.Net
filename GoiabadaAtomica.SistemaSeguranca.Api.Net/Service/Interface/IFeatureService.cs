using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Request.Feature;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Interface
{
    public interface IFeatureService
    {
        Task<FeatureDTO> CreateFeatureAsync(int tenantId, int clientSystemId, CreateFeatureRequestDTO createFeatureRequestDTO);
        Task<FeatureDTO?> UpdateFeatureAsync(int tenantId, int clientSystemId, int featureId, UpdateFeatureRequestDTO updateFeatureRequestDTO);
        Task<FeatureDTO?> DeactivateActivateFeatureAsync(int tenantId, int clientSystemId, int featureId, bool newStatus);
        Task<IEnumerable<FeatureDTO?>> GetAllFeaturesByClientSystemIdAsync(int tenantId, int clientSystemId);
        Task<FeatureDTO?> GetFeatureByIdAsync(int tenantId, int clientSystemId, int featureId);
        Task<bool> HasActiveFeaturesAsync(int clientSystemId);
    }
}
