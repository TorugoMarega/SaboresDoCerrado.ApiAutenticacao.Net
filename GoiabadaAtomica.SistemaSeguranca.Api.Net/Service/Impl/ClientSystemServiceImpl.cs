using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Request.ClientSystem;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response.ClientSystem;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.Entity;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Interface;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Interface;
using Mapster;
using System.Security.Cryptography;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Impl
{
    public class ClientSystemServiceImpl : IClientSystemService
    {
        private readonly IClientSystemRepository _clientSystemRepository;
        private readonly IFeatureService _featureService;
        private readonly IAuthenticationProviderRepository _authenticationProviderRepository;
        private readonly ILogger<ClientSystemServiceImpl> _logger;

        public ClientSystemServiceImpl(IClientSystemRepository clientSystemRepository, ILogger<ClientSystemServiceImpl> logger, IFeatureService featureService, IAuthenticationProviderRepository authenticationProviderRepository)
        {
            _clientSystemRepository = clientSystemRepository;
            _logger = logger;
            _featureService = featureService;
            _authenticationProviderRepository = authenticationProviderRepository;
        }

        public async Task<CreateClientSystemResponseDTO> CreateClientSystemAsync(int tenantId,CreateClientSystemRequestDTO createClientSystemRequestDTO)
        {
            _logger.LogInformation("Validando o nome do sistema");
            if (await _clientSystemRepository.ExistsClientSystemByNameAsync(tenantId,createClientSystemRequestDTO.Name))
            {
                throw new InvalidOperationException($"O nome [{createClientSystemRequestDTO.Name}] já está em uso por outro sistema já cadastrado anteriormente!");
            }

            var clientSystemEntity = createClientSystemRequestDTO.Adapt<ClientSystemEntity>();
            clientSystemEntity.CreatedAt = DateTime.UtcNow;
            clientSystemEntity.UpdatedAt = DateTime.UtcNow;
            clientSystemEntity.IsActive = true;
            _logger.LogInformation("Gerando client ID");
            clientSystemEntity.ClientId = Guid.NewGuid().ToString();


            var randomNumber = RandomNumberGenerator.Create();
            var randomBytes = new byte[32];

            _logger.LogInformation("Gerando Secret");
            randomNumber.GetBytes(randomBytes);

            //iremos devolver ao admin uma única vez após o cadastro
            string clientSecretPlainText = Convert.ToBase64String(randomBytes);

            clientSystemEntity.ClientSecret = BCrypt.Net.BCrypt.HashPassword(clientSecretPlainText);

            var createClientSystemResponseDTO = await _clientSystemRepository.CreateClientSystemAsync(clientSystemEntity);
            createClientSystemResponseDTO.ClientSecret = clientSecretPlainText;
            _logger.LogInformation("O Sistema [{SystemName}] foi criado com sucesso. ClientId: [{ClientId}]", createClientSystemResponseDTO.Name, createClientSystemResponseDTO.ClientId);
            return createClientSystemResponseDTO;
        }
        public async Task<IEnumerable<ClientSystemDTO>> GetAllClientSystemAsync(int tenantId)
        {
            _logger.LogInformation("Iniciando listagem de Sistemas");
            return await _clientSystemRepository.GetAllClientSystemByAsync(tenantId);
        }
        public async Task<ClientSystemDTO?> GetClientSystemByIdAsync(int tenantId, int clientSystemId)
        {
            var clientSystem = await _clientSystemRepository.GetClientSystemDTOByIdAsync(tenantId, clientSystemId);
            if (clientSystem == null)
            {
                return null;
            }
            return clientSystem;
        }
        public async Task<ClientSystemDTO?> UpdateClientSystemAsync(int tenantId,int clientSystemId, UpdateClientSystemRequestDTO updateClientSystemRequestDTO)
        {
            _logger.LogInformation("Buscando Sistema [{ID}] no banco de dados", clientSystemId);

            var nameInUse = await _clientSystemRepository.ExistsClientSystemByNameAsync(tenantId, updateClientSystemRequestDTO.Name);
            if (nameInUse)
            {
                throw new InvalidOperationException($"O nome {updateClientSystemRequestDTO.Name} já está em uso");
            }

            var clientSystemEntity = await _clientSystemRepository.GetClientSystemEntityByIdAsync(tenantId, clientSystemId);
            if (clientSystemEntity == null)
            {
                return null;
            }

            clientSystemEntity.Name = updateClientSystemRequestDTO.Name;
            clientSystemEntity.Description = updateClientSystemRequestDTO.Description;

            //validacoes
            await ValidationDeactivateActivateClientSystemAsync(clientSystemId, updateClientSystemRequestDTO.IsActive);

            clientSystemEntity.IsActive = updateClientSystemRequestDTO.IsActive;

            _logger.LogInformation("Atualizando dados do Sistema [{ID}] no banco de dados", clientSystemId);
            await _clientSystemRepository.UpdateClientSystemAsync(clientSystemEntity);
            _logger.LogInformation("Retornando Sistema [{ID}] no banco de dados", clientSystemId);
            return await _clientSystemRepository.GetClientSystemDTOByIdAsync(tenantId, clientSystemId);
        }
        public async Task<bool?> DeactivateActivateClientSystemAsync(int tenantId, int clientSystemId, bool newStatus)
        {
            _logger.LogInformation("Buscando Sistema [{ID}] no banco de dados", clientSystemId);
            var clientSystemEntity = await _clientSystemRepository.GetClientSystemEntityByIdAsync(tenantId, clientSystemId);
            if (clientSystemEntity == null)
            {
                return null;
            }

            //regras de validacao
            await ValidationDeactivateActivateClientSystemAsync(clientSystemId, newStatus);

            clientSystemEntity.IsActive = newStatus;

            await _clientSystemRepository.UpdateClientSystemAsync(clientSystemEntity);
            return true;
        }
        public async Task<bool> ExistsClientSystemByIdAsync(int tenantId, int clientSystemId)
        {
            _logger.LogInformation("Verificando existência do Sistema [{ClientSystemId}]", clientSystemId);
            return await _clientSystemRepository.ExistsClientSystemById(tenantId, clientSystemId);
        }
        private async Task ValidationDeactivateActivateClientSystemAsync(int clientSystemId, bool newStatus)
        {
            _logger.LogDebug("Executando validação de dependências para o ClientSystem ID: [{ClientSystemiD}]", clientSystemId);

            // A validação só acontece se estivermos tentando INATIVAR o sistema.
            if (newStatus == false)
            {
                // Validação 1: Provedores de Autenticação
                if (await _authenticationProviderRepository.HasActiveAuthenticationProviderByClientSystemIdAsync(clientSystemId))
                {
                    throw new InvalidOperationException("Este sistema não pode ser inativado pois possui provedores de autenticação ativos associados.");
                }

                // Validação 2: Funcionalidades (Features)
                if (await _featureService.HasActiveFeaturesAsync(clientSystemId))
                {
                    throw new InvalidOperationException("Este sistema não pode ser inativado pois possui funcionalidades ativas.");
                }
            }
        }
    }
}
