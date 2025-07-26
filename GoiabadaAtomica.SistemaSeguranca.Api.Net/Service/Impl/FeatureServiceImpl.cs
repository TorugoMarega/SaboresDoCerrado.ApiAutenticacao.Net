using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Request.Feature;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.Entity;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Impl;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Interface;
using Mapster;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Impl
{
    public class FeatureServiceImpl:IFeatureService
    {
        private readonly ILogger<IFeatureService> _logger;
        private readonly FeatureRepositoryImpl _featureRepository;
        private readonly ClientSystemRepositoryImpl _clientSystemRepository;

        public FeatureServiceImpl(ILogger<IFeatureService> logger, FeatureRepositoryImpl featureRepository)
        {
            _logger = logger;
            _featureRepository = featureRepository;
        }

        public async Task<FeatureDTO> CreateFeatureAsync(CreateFeatureRequestDTO createFeatureRequestDTO){
            _logger.LogInformation("Iniciando cadastro da Feature [{Name}]", createFeatureRequestDTO.Name);
            _logger.LogInformation("Verificando existencia do Sistema ID [{ClientSystemID}]", createFeatureRequestDTO.ClientSystemId);
            if(!await _clientSystemRepository.ExistsClientSystemById(createFeatureRequestDTO.ClientSystemId))
            {
                var msg = $"Não existe sistema cadastrado com o ID: [{createFeatureRequestDTO.ClientSystemId}]";
                _logger.LogWarning(msg);
                throw new InvalidOperationException(msg);
            }
            if (await _featureRepository.ExistsActiveFeatureByClientSystemIdAsync(createFeatureRequestDTO.ClientSystemId)) {
                var msg = $"Já existe Feature cadastrado com o nome [{createFeatureRequestDTO.Name}] para o sistema [{createFeatureRequestDTO.ClientSystemId}]";
                _logger.LogWarning(msg);
                throw new InvalidOperationException(msg);
            }
            return await _featureRepository.CreateFeatureAsync(createFeatureRequestDTO.Adapt<FeatureEntity>());
        }
        public async Task<FeatureDTO?> UpdateFeatureAsync(int id, UpdateFeatureRequestDTO updateFeatureRequestDTO) {
            throw new NotImplementedException();
        }
        public async Task<bool?> DeactivateActivateFeatureAsync(int id, bool newStatus){
        throw new NotImplementedException();}
        public async Task<IEnumerable<FeatureDTO>> GetAllFeaturesAsync(){
            _logger.LogInformation("Iniciando listagem de Features");
            throw new NotImplementedException();}
        public async Task<FeatureDTO?> GetFeatureByIdAsync(int id){
        throw new NotImplementedException();}
    }
}
