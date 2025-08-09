using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Request.Tenant;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response.Tenant;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.Entity;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Interface;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Interface;
using Mapster;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Service.Impl
{
    public class TenantServiceImpl : ITenantService
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly ILogger<ITenantService> _logger;

        public TenantServiceImpl(ITenantRepository tenantRepository, ILogger<ITenantService> logger)
        {
            _logger = logger;
            _tenantRepository = tenantRepository;
        }

        public async Task<TenantDTO> CreateTenantAsync(CreateTenantRequestDTO createTenantRequestDTO)
        {
            _logger.LogInformation("Validando o nome do empresa");
            if (await _tenantRepository.ExistsTenantByNameAsync(createTenantRequestDTO.Name))
            {
                throw new InvalidOperationException($"O nome [{createTenantRequestDTO.Name}] já está em uso por outra empresa já cadastrada anteriormente!");
            }

            var tenantEntity = createTenantRequestDTO.Adapt<TenantEntity>();
            tenantEntity.CreatedAt = DateTime.UtcNow;
            tenantEntity.UpdatedAt = DateTime.UtcNow;
            tenantEntity.IsActive = true;


            var TenantDTO = await _tenantRepository.CreateTenantAsync(tenantEntity);
            _logger.LogInformation("A empresa [{Empresa}] foi criada com sucesso.", TenantDTO.Name);
            return TenantDTO;
        }
        public async Task<IEnumerable<TenantDTO>> GetAllTenantAsync()
        {
            _logger.LogInformation("Iniciando listagem de Empresas");
            return await _tenantRepository.GetAllTenantByAsync();
        }
        public async Task<TenantDTO?> GetTenantByIdAsync(int id)
        {
            var tenant = await _tenantRepository.GetTenantDTOByIdAsync(id);
            if (tenant == null)
            {
                return null;
            }
            return tenant;
        }
        public async Task<TenantDTO?> UpdateTenantAsync(int id, UpdateTenantRequestDTO updateTenantRequestDTO)
        {
            _logger.LogInformation("Buscando Sistema [{ID}] no banco de dados", id);

            var nameInUse = await _tenantRepository.ExistsTenantByNameIdExcludeIdAsync(updateTenantRequestDTO.Name, id);
            if (nameInUse)
            {
                throw new InvalidOperationException($"O nome {updateTenantRequestDTO.Name} já está em uso por outra Empresa");
            }

            var tenantEntity = await _tenantRepository.GetTenantEntityByIdAsync(id);
            if (tenantEntity == null)
            {
                return null;
            }

            tenantEntity.Name = updateTenantRequestDTO.Name;
            tenantEntity.ContactPhone = updateTenantRequestDTO.ContactPhone;
            tenantEntity.ContactEmail = updateTenantRequestDTO.ContactEmail;
            tenantEntity.UpdatedAt = DateTime.UtcNow;

            //validacoes
            ValidationDeactivateActivateTenant(tenantEntity, updateTenantRequestDTO.IsActive);

            tenantEntity.IsActive = updateTenantRequestDTO.IsActive;
            _logger.LogInformation("Atualizando dados da Empresa [{ID}] no banco de dados", id);
            await _tenantRepository.UpdateTenantAsync(tenantEntity);
            _logger.LogInformation("Retornando Empresa [{ID}] no banco de dados", id);
            return await _tenantRepository.GetTenantDTOByIdAsync(id);
        }
        public async Task<bool?> DeactivateActivateTenantAsync(int id, bool newStatus)
        {
            _logger.LogInformation("Buscando Empresa [{ID}] no banco de dados", id);
            var tenantEntity = await _tenantRepository.GetTenantEntityByIdAsync(id);
            if (tenantEntity == null)
            {
                return null;
            }

            //regras de validacao
            ValidationDeactivateActivateTenant(tenantEntity, newStatus);

            tenantEntity.IsActive = newStatus;

            await _tenantRepository.UpdateTenantAsync(tenantEntity);
            return true;
        }
        public async Task<bool> ExistsTenantByIdAsync(int tenantId)
        {
            _logger.LogInformation("Verificando existência da Empresa [{TenantId}]", tenantId);
            return await _tenantRepository.ExistsTenantById(tenantId);
        }
        private void ValidationDeactivateActivateTenant(TenantEntity tenantEntity, bool newStatus)
        {
            _logger.LogDebug("Executando validação para a empresa: [{TenantID}]", tenantEntity.Id);
            if (tenantEntity.IsActive == newStatus)
            {
                var statusText = newStatus ? "ativa" : "inativa";
                _logger.LogWarning("Tentativa de alterar o status da Empresa ID [{EmpresaId}], mas ela já está [{Status}]", tenantEntity.Id, statusText);
                throw new InvalidOperationException($"Esta Empresa já está {statusText}.");
            }
        }
    }
}
