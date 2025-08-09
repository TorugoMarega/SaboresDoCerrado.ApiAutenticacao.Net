using GoiabadaAtomica.ApiAutenticacao.Net.Data;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response.Tenant;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.Entity;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Interface;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Impl
{
    public class TenantRepositoryImpl : ITenantRepository
    {
        private readonly ApplicationContext _applicationContext;
        private readonly ILogger<TenantRepositoryImpl> _logger;

        public TenantRepositoryImpl(ApplicationContext applicationContext, ILogger<TenantRepositoryImpl> logger)
        {
            _applicationContext = applicationContext;
            _logger = logger;
        }

        public async Task<TenantEntity?> GetTenantEntityByIdAsync(int id)
        {
            _logger.LogInformation("Iniciando busca da Empresa pelo id [{id}]", id);

            var entity = await _applicationContext.TenantEntity
                .FirstOrDefaultAsync(tenant => tenant.Id == id);

            _logger.LogDebug("Busca de Empresa finalizada com sucesso!");
            return entity;
        }
        public async Task<TenantDTO?> GetTenantDTOByIdAsync(int id)
        {
            _logger.LogInformation("Iniciando busca de Empresa pelo id [{id}]", id);

            var tenantDTO = await _applicationContext.TenantEntity
                .AsNoTracking()
                .Select(tenant => new TenantDTO
                {
                    Id = tenant.Id,
                    Name = tenant.Name,
                    ContactEmail = tenant.ContactEmail,
                    ContactPhone = tenant.ContactPhone,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = tenant.UpdatedAt,
                    IsActive = tenant.IsActive
                })
                .FirstOrDefaultAsync(tenant => tenant.Id == id);
            _logger.LogDebug("Busca de Empresa finalizada com sucesso!");
            return tenantDTO;
        }
        public async Task<IEnumerable<TenantDTO>> GetAllTenantByAsync()
        {
            _logger.LogInformation("Iniciando busca de todas as Empresas");
            var tenantDTOList = await _applicationContext.TenantEntity
                .AsNoTracking()
                .Select(tenant => new TenantDTO
                {
                    Id = tenant.Id,
                    Name = tenant.Name,
                    ContactEmail = tenant.ContactEmail,
                    ContactPhone = tenant.ContactPhone,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = tenant.UpdatedAt,
                    IsActive = tenant.IsActive
                })
                .ToListAsync();
            _logger.LogDebug("Busca de Empresas finalizada com sucesso!");
            return tenantDTOList;
        }
        public async Task<TenantDTO> CreateTenantAsync(TenantEntity tenant)
        {
            _logger.LogDebug("Persistindo Empresa no banco de dados");
            await _applicationContext.TenantEntity.AddAsync(tenant);
            await _applicationContext.SaveChangesAsync();
            return tenant.Adapt<TenantDTO>();
        }
        public async Task<int> UpdateTenantAsync(TenantEntity tenant)
        {
            _logger.LogDebug("Atualizando Empresa no banco de dados");
            _applicationContext.TenantEntity.Update(tenant);
            return await _applicationContext.SaveChangesAsync();
        }
        public async Task<bool> ExistsTenantById(int id)
        {
            _logger.LogDebug("Validando existencia da Empresa no banco de dados");
            var exists = await _applicationContext.TenantEntity
                .AsNoTracking()
                .AnyAsync(tenant => tenant.Id.Equals(id));
            return exists;
        }
        public async Task<bool> ExistsTenantByNameAsync(string name)
        {
            _logger.LogDebug("Validando existencia da empresa no banco de dados");
            var exists = await _applicationContext.TenantEntity
                .AsNoTracking()
                .AnyAsync(tenant => tenant.Name.ToLower().Equals(name.ToLower()));
            return exists;
        }

        public async Task<bool> ExistsTenantByNameIdExcludeIdAsync(string name, int id)
        {
            _logger.LogDebug("Validando existencia do nome de empresa no banco de dados");
            var exists = await _applicationContext.TenantEntity
                .AsNoTracking()
                .AnyAsync(tenant => tenant.Name.Equals(name) && !tenant.Id.Equals(id));
            return exists;
        }
    }
}
