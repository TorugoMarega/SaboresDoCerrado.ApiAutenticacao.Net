using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.Entity;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Interface
{
    public interface IFeatureRepository
    {
        Task<FeatureEntity?> GetFeatureEntityByIdAsync(int clientSystemId, int featureId);
        Task<FeatureDTO?> GetFeatureDTOByIdAsync(int clientSystemId, int featureId);
        Task<IEnumerable<FeatureDTO>> GetAllFeaturesAsync(int clientSystemId);
        Task<FeatureDTO> CreateFeatureAsync(FeatureEntity featureEntity);
        Task<int> UpdateFeatureAsync(FeatureEntity featureEntity);
        Task<bool> DoesNameExistForSystemAsync(int clientSystemId, string featureName, int? featureIdToExclude);
        Task<bool> HasActiveFeaturesAsync(int clientSystemId);
    }
}
