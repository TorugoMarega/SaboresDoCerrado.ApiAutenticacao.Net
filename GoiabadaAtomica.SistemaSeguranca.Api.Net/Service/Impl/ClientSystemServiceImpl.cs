using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Request.ClientSystem;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response.ClientSystem;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.Entity;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Impl;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Interface;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Interface;
using Mapster;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Impl
{
    public class ClientSystemServiceImpl : IClientSystemService
    {
        private readonly ClientSystemRepositoryImpl _clientSystemRepository;
        private readonly IFeatureRepository _featureRepository;
        private readonly IAuthenticationProviderRepository _authenticationProviderRepository;
        private readonly ILogger<ClientSystemRepositoryImpl> _logger;

        public ClientSystemServiceImpl(ClientSystemRepositoryImpl clientSystemRepository, ILogger<ClientSystemRepositoryImpl> logger, IFeatureRepository featureRepository, IAuthenticationProviderRepository authenticationProviderRepository)
        {
            _clientSystemRepository = clientSystemRepository;
            _logger = logger;
            _featureRepository = featureRepository;
            _authenticationProviderRepository = authenticationProviderRepository;
        }

        public async Task<CreateClientSystemResponseDTO> CreateClientSystemAsync(CreateClientSystemRequestDTO createClientSystemRequestDTO)
        {
            _logger.LogInformation("Validando o nome do sistema");
            if (await _clientSystemRepository.ExistsClientSystemByNameAsync(createClientSystemRequestDTO.Name))
            {
                throw new InvalidOperationException($"O nome [{createClientSystemRequestDTO.Name}] já está em uso por outro sistema já cadastrado anteriormente!");
            }

            var clientSystemEntity = createClientSystemRequestDTO.Adapt<ClientSystemEntity>();
            clientSystemEntity.CreatedAt = DateTime.UtcNow;
            clientSystemEntity.UpdatedAt = DateTime.UtcNow;
            clientSystemEntity.IsActive = true;
            clientSystemEntity.ClientId = Guid.NewGuid().ToString();

            //iremos devolver ao admin uma única vez após o cadastro
            var secretId = Guid.NewGuid().ToString();
            clientSystemEntity.ClientSecret = BCrypt.Net.BCrypt.HashPassword(secretId);

            var createClientSystemResponseDTO = await _clientSystemRepository.CreateClientSystemAsync(clientSystemEntity);
            createClientSystemResponseDTO.ClientSecret = secretId;

            return createClientSystemResponseDTO;
        }
        public async Task<IEnumerable<ClientSystemDTO>> GetAllClientSystemAsync()
        {
            _logger.LogInformation("Iniciando listagem de Sistemas");
            return await _clientSystemRepository.GetAllClientSystemByAsync();
        }
        public async Task<ClientSystemDTO?> GetClientSystemByIdAsync(int id)
        {
            var clientSystem = await _clientSystemRepository.GetClientSystemDTOByIdAsync(id);
            if (clientSystem == null)
            {
                return null;
            }
            return clientSystem;
        }
        public async Task<ClientSystemDTO?> UpdateClientSystemAsync(int id, UpdateClientSystemRequestDTO updateClientSystemRequestDTO)
        {
            _logger.LogInformation("Buscando Sistema [{ID}] no banco de dados", id);

            var nameInUse = await _clientSystemRepository.ExistsClientSystemByNameAsync(updateClientSystemRequestDTO.Name);
            if (nameInUse)
            {
                throw new InvalidOperationException($"O nome {updateClientSystemRequestDTO.Name} já está em uso");
            }

            var clientSystemEntity = await _clientSystemRepository.GetClientSystemEntityByIdAsync(id);
            if (clientSystemEntity == null)
            {
                return null;
            }

            clientSystemEntity.Name = updateClientSystemRequestDTO.Name;
            clientSystemEntity.Description = updateClientSystemRequestDTO.Description;

            //validacoes
            await ValidationDeactivateActivateClientSystemAsync(id, updateClientSystemRequestDTO.IsActive);

            clientSystemEntity.IsActive = updateClientSystemRequestDTO.IsActive;
            _logger.LogInformation("Atualizando dados do Sistema [{ID}] no banco de dados", id);
            await _clientSystemRepository.UpdateClientSystemAsync(clientSystemEntity);
            _logger.LogInformation("Retornando Sistema [{ID}] no banco de dados", id);
            return await _clientSystemRepository.GetClientSystemDTOByIdAsync(id);
        }
        public async Task<bool?> DeactivateActivateClientSystemAsync(int id, bool newStatus)
        {
            _logger.LogInformation("Buscando Sistema [{ID}] no banco de dados", id);
            var clientSystemEntity = await _clientSystemRepository.GetClientSystemEntityByIdAsync(id);
            if (clientSystemEntity == null)
            {
                return null;
            }

            //regras de validacao
            await ValidationDeactivateActivateClientSystemAsync(id, newStatus);

            clientSystemEntity.IsActive = newStatus;

            await _clientSystemRepository.UpdateClientSystemAsync(clientSystemEntity);
            return true;
        }

        private async Task ValidationDeactivateActivateClientSystemAsync(int clientSystemId, bool newStatus)
        {
            _logger.LogDebug("Executando validação de dependências para o ClientSystem ID: [{Id}]", clientSystemId);
            if (newStatus.Equals(false))
            {
                if(await _authenticationProviderRepository.ExistsActiveAuthenticationProviderByClientSystemIdAsync(clientSystemId))
                {
                    throw new InvalidOperationException("Este sistema não pode ser inativado pois possui funcionalidades ativas.");
                }

                if (await _featureRepository.ExistsActiveFeatureByClientSystemIdAsync(clientSystemId))
                {
                    throw new InvalidOperationException("Este sistema não pode ser inativado pois possui provedores de autenticação ativos associados.");
                }
            }
        }
    }
}
