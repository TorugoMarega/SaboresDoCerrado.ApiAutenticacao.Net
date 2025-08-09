using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Request.Feature;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.Entity;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Impl;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Interface;
using Mapster;
using IClientSystemService = GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Interface.IClientSystemService;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Impl
{
    public class FeatureServiceImpl : IFeatureService
    {
        private readonly ILogger<IFeatureService> _logger;
        private readonly FeatureRepositoryImpl _featureRepository;
        private readonly IClientSystemService _clientSystemService;

        public FeatureServiceImpl(ILogger<IFeatureService> logger, FeatureRepositoryImpl featureRepository, IClientSystemService clientSystemService)
        {
            _logger = logger;
            _featureRepository = featureRepository;
            _clientSystemService = clientSystemService;
        }

        public async Task<FeatureDTO> CreateFeatureAsync(int clientSystemId, CreateFeatureRequestDTO createFeatureRequestDTO)
        {
            _logger.LogInformation("Iniciando cadastro da Feature [{Name}]", createFeatureRequestDTO.Name);
            _logger.LogInformation("Verificando existencia do Sistema ID [{ClientSystemID}]", clientSystemId);
            if (!await _clientSystemService.ExistsClientSystemByIdAsync(clientSystemId))
            {
                var msg = $"Não existe sistema cadastrado com o ID: [{clientSystemId}]";
                _logger.LogWarning(msg);
                throw new InvalidOperationException(msg);
            }
            if (await _featureRepository.DoesNameExistForSystemAsync(clientSystemId, createFeatureRequestDTO.Name))
            {
                var msg = $"Já existe Feature cadastrado com o nome [{createFeatureRequestDTO.Name}] para o sistema [{clientSystemId}]";
                _logger.LogWarning(msg);
                throw new InvalidOperationException(msg);
            }
            var newEntity = createFeatureRequestDTO.Adapt<FeatureEntity>();
            newEntity.ClientSystemId = clientSystemId;

            return await _featureRepository.CreateFeatureAsync(newEntity);
        }
        public async Task<FeatureDTO?> UpdateFeatureAsync(int clientSystemId, int featureId, UpdateFeatureRequestDTO updateFeatureRequestDTO)
        {
            _logger.LogInformation("Iniciando processo de atualização da Feature [{FeatureId}] - [{Name}] do sistema [{clientSystemId}]", featureId, updateFeatureRequestDTO.Name, clientSystemId);
            if (await _featureRepository.DoesNameExistForSystemAsync(clientSystemId, updateFeatureRequestDTO.Name, featureId))
            {
                _logger.LogWarning("Já existe uma feature cadastrada com o mesmo nome no sistema [{clientSystemId}]", clientSystemId);
                throw new InvalidOperationException("Nome da funcionalidade já existe para este sistema.");
            }

            var featureEntity = await _featureRepository.GetFeatureEntityByIdAsync(clientSystemId, featureId);
            if (featureEntity is null)
            {
                _logger.LogWarning("Não foi encontrada nenhuma feature cadastrada com o ID: [{FeatureId}] para o sistema [{clientSystemId}]", featureId, clientSystemId);
                return null;
            }

            if (featureEntity.IsActive != updateFeatureRequestDTO.IsActive)
            {
                //Valida e altera o status caso seja possível
                ValidateAndApplyStatusChange(featureEntity, updateFeatureRequestDTO.IsActive);
            }

            featureEntity.Name = updateFeatureRequestDTO.Name;
            featureEntity.Description = updateFeatureRequestDTO.Description;

            _logger.LogInformation("Tentando persistir informações no banco de dados");
            await _featureRepository.UpdateFeatureAsync(featureEntity);
            return featureEntity.Adapt<FeatureDTO>();
        }
        public async Task<FeatureDTO?> DeactivateActivateFeatureAsync(int clientSystemId, int featureId, bool newStatus)
        {
            _logger.LogInformation("Iniciando processo de desatiovação da Feature [{FeatureId}] do sistema [{clientSystemId}]", featureId, clientSystemId);

            var featureEntity = await _featureRepository.GetFeatureEntityByIdAsync(clientSystemId, featureId);
            if (featureEntity is null || featureEntity.ClientSystemId != clientSystemId)
            {
                _logger.LogWarning("Não foi encontrada nenhuma feature [{FeatureId}] cadastrada para o sistema [{clientSystemId}]", featureId, clientSystemId);
                throw new KeyNotFoundException("Funcionalidade não encontrada ou não pertence a este sistema.");
            }

            ValidateAndApplyStatusChange(featureEntity, newStatus);

            _logger.LogInformation("Tentando persistir mudança de status no banco de dados");
            await _featureRepository.UpdateFeatureAsync(featureEntity);
            return featureEntity.Adapt<FeatureDTO>();
        }
        private void ValidateAndApplyStatusChange(FeatureEntity feature, bool newStatus)
        {
            // Regra 1: Não fazer nada se o status já for o desejado.
            if (feature.IsActive == newStatus)
            {
                var statusText = newStatus ? "ativa" : "inativa";
                _logger.LogWarning("Tentativa de alterar o status da Feature ID [{FeatureId}], mas ela já está [{Status}]", feature.Id, statusText);
                throw new InvalidOperationException($"Esta funcionalidade já está {statusText}.");
            }

            // Regra 2: Se a intenção é DESATIVAR, checamos as dependências.
            if (newStatus == false)
            {
                _logger.LogInformation("Validação de dependências para desativação da Feature ID [{FeatureId}]", feature.Id);
                if (feature.RoleFeature.Any()) // Assumindo que a feature foi carregada com .Include(f => f.Roles)
                {
                    _logger.LogWarning("Falha na desativação: Feature ID [{FeatureId}] está em uso por perfis.", feature.Id);
                    throw new InvalidOperationException("Não é possível desativar uma funcionalidade que está em uso por um ou mais perfis.");
                }
            }

            // Se todas as validações passaram, aplica o novo status.
            _logger.LogInformation("Alterando status da Feature ID [{FeatureId}] para {NewStatus}", feature.Id, newStatus);
            feature.IsActive = newStatus;
        }
        public async Task<IEnumerable<FeatureDTO?>> GetAllFeaturesByClientSystemIdAsync(int clientSystemId)
        {
            _logger.LogInformation("Iniciando listagem de Features do sistema [{ClientSystemId}]", clientSystemId);
            return await _featureRepository.GetAllFeaturesAsync(clientSystemId);
        }
        public async Task<FeatureDTO?> GetFeatureByIdAsync(int clientSystemId, int featureId)
        {
            _logger.LogInformation("Iniciando processo de busca da feature [{FeatureId}] para o sistema [{clientSystemId}]", featureId, clientSystemId);
            var featureDTO = await _featureRepository.GetFeatureDTOByIdAsync(clientSystemId, featureId);
            if (featureDTO is null)
            {
                _logger.LogWarning("A feature [{FeatureId}] não foi encontrada na base", featureId);
            }
            return featureDTO;
        }
        public async Task<bool> HasActiveFeaturesAsync(int clientSystemId)
        {
            _logger.LogInformation("Verificando existência de alguma Feature ativa para o sistema [{ClientSystemId}]", clientSystemId);
            return await _featureRepository.HasActiveFeaturesAsync(clientSystemId);
        }
    }
}
