using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.Entity;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Interface
{
    public interface IFeatureRepository
    {
        Task<FeatureEntity?> GetFeatureEntityByIdAsync(int id);
        Task<FeatureDTO?> GetFeatureDTOByIdAsync(int id);
        Task<IEnumerable<FeatureDTO>> GetAllFeatureByAsync();
        Task<FeatureDTO> CreateFeatureAsync(FeatureEntity featureEntity);
        Task<int> UpdateFeatureAsync(FeatureEntity featureEntity);
        Task<bool> ExistsFeatureById(int id);
        Task<bool> ExistsFeatureByNameAsync(string name);
        Task<bool> ExistsActiveFeatureByClientSystemIdAsync(int clientSystemId);
    }
}
