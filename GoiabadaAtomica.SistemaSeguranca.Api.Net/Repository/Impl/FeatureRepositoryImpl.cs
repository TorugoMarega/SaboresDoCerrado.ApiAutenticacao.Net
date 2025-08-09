using GoiabadaAtomica.ApiAutenticacao.Net.Data;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.Entity;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Interface;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Impl
{
    public class FeatureRepositoryImpl : IFeatureRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<FeatureRepositoryImpl> _logger;

        public FeatureRepositoryImpl(ApplicationContext context, ILogger<FeatureRepositoryImpl> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<FeatureEntity?> GetFeatureEntityByIdAsync(int clientSystemId, int featureId)
        {
            _logger.LogDebug("Iniciando busca de Feature pelo id [{id}]", featureId);

            var entity = await _context.FeatureEntity
                .FirstOrDefaultAsync(feature => feature.Id == featureId && feature.ClientSystemId == clientSystemId);

            _logger.LogDebug("Busca de Feature finalizada com sucesso!");
            return entity;
        }
        public async Task<FeatureDTO?> GetFeatureDTOByIdAsync(int clientSystemId, int featureId)
        {
            _logger.LogDebug("Iniciando busca de Feature [{featureId}] do sistema [{clientSystemId}]", featureId, clientSystemId);

            var featureDTO = await _context.FeatureEntity
                .AsNoTracking()
                .Select(feature => new FeatureDTO
                {
                    Id = feature.Id,
                    Name = feature.Name,
                    Description = feature.Description,
                    ClientSystemId = feature.ClientSystemId,
                    IsActive = feature.IsActive
                })
                .FirstOrDefaultAsync(feature => feature.Id == featureId && feature.ClientSystemId == clientSystemId);
            _logger.LogDebug("Busca de Feature finalizada com sucesso!");
            return featureDTO;
        }
        public async Task<IEnumerable<FeatureDTO>> GetAllFeaturesAsync(int clientSystemId)
        {
            _logger.LogDebug("Iniciando busca no banco de todas as Features do sistema [{clientSystemId}]", clientSystemId);
            var featureDTOList = await _context.FeatureEntity
                .AsNoTracking()
                .Where(f => f.ClientSystemId == clientSystemId)
                .Select(feature => new FeatureDTO
                {
                    Id = feature.Id,
                    Name = feature.Name,
                    Description = feature.Description,
                    ClientSystemId = feature.ClientSystemId,
                    IsActive = feature.IsActive
                })
                .ToListAsync();
            _logger.LogDebug("Busca de Features do sistema [{clientSystemId}] finalizada com sucesso!", clientSystemId);
            return featureDTOList;
        }
        public async Task<FeatureDTO> CreateFeatureAsync(FeatureEntity feature)
        {
            _logger.LogDebug("Persistindo Feature no banco de dados");
            await _context.FeatureEntity.AddAsync(feature);
            await _context.SaveChangesAsync();
            return feature.Adapt<FeatureDTO>();
        }
        public async Task<int> UpdateFeatureAsync(FeatureEntity feature)
        {
            _logger.LogDebug("Atualizando Features no banco de dados");
            _context.FeatureEntity.Update(feature);
            return await _context.SaveChangesAsync();
        }
        public async Task<bool> DoesNameExistForSystemAsync(int clientSystemId, string featureName, int? featureIdToExclude = null)
        {
            _logger.LogDebug("Validando existencia de Features com o nome [{Name}] para o [{Sistema}] no banco de dados", featureName, clientSystemId);
            var query = _context.FeatureEntity
                .AsNoTracking()
                .Where(feat => feat.Name.ToLower() == featureName.ToLower() && feat.ClientSystemId == clientSystemId);
            //validacao no update
            if (featureIdToExclude.HasValue)
            {
                query = query.Where(f => f.Id != featureIdToExclude.Value);
            }
            return await query.AnyAsync();
        }
        public async Task<FeatureEntity?> GetByIdWithRolesAsync(int featureId)
        {
            _logger.LogDebug("Buscando Feature ID [{FeatureId}] incluindo seus vinculos com Roles", featureId);
            return await _context.FeatureEntity
                .Include(f => f.RoleFeature)
                .FirstOrDefaultAsync(f => f.Id == featureId);
        }
        public async Task<bool> HasActiveFeaturesAsync(int clientSystemId)
        {
            _logger.LogDebug("Validando se há alguma feature ativa para o sistema [{ClientSystemId}]", clientSystemId);
            return await _context.FeatureEntity
                .AnyAsync(feature => feature.ClientSystemId == clientSystemId && feature.IsActive == true);
        }
    }
}
