using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Request.Feature;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Interface
{
    public interface IFeatureService
    {
        Task<FeatureDTO> CreateFeatureAsync(CreateFeatureRequestDTO createFeatureRequestDTO);
        Task<FeatureDTO?> UpdateFeatureAsync(int id, UpdateFeatureRequestDTO updateFeatureRequestDTO);
        Task<bool?> DeactivateActivateFeatureAsync(int id, bool newStatus);
        Task<IEnumerable<FeatureDTO>> GetAllFeaturesAsync();
        Task<FeatureDTO?> GetFeatureByIdAsync(int id);
    }
}
