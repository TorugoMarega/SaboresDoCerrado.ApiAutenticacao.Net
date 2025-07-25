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
        private readonly ILogger<IFeatureRepository> _logger;

        public FeatureRepositoryImpl(ApplicationContext context, ILogger<IFeatureRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<FeatureEntity?> GetFeatureEntityByIdAsync(int id)
        {
            _logger.LogDebug("Iniciando busca de Feature pelo id [{id}]", id);

            var entity = await _context.FeatureEntity
                .FirstOrDefaultAsync(feature => feature.Id == id);

            _logger.LogDebug("Busca de Feature finalizada com sucesso!");
            return entity;
        }
        public async Task<FeatureDTO?> GetFeatureDTOByIdAsync(int id)
        {
            _logger.LogDebug("Iniciando busca de Feature pelo id [{id}]", id);

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
                .FirstOrDefaultAsync(feature => feature.Id == id);
            _logger.LogDebug("Busca de Feature finalizada com sucesso!");
            return featureDTO;
        }
        public async Task<IEnumerable<FeatureDTO>> GetAllFeatureByAsync()
        {
            _logger.LogDebug("Iniciando busca de todas as Features");
            var featureDTOList = await _context.FeatureEntity
                .AsNoTracking()
                .Select(feature => new FeatureDTO
                {
                    Id = feature.Id,
                    Name = feature.Name,
                    Description = feature.Description,
                    ClientSystemId = feature.ClientSystemId,
                    IsActive = feature.IsActive
                })
                .ToListAsync();
            _logger.LogDebug("Busca de Features finalizada com sucesso!");
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
        public async Task<bool> ExistsFeatureById(int id)
        {
            _logger.LogDebug("Validando existencia da Features no banco de dados");
            var exists = await _context.FeatureEntity
                .AsNoTracking()
                .AnyAsync(feature => feature.Id.Equals(id));
            return exists;
        }
        public async Task<bool> ExistsFeatureByNameAsync(string name)
        {
            _logger.LogDebug("Validando existencia da Features no banco de dados");
            var exists = await _context.FeatureEntity
                .AsNoTracking()
                .AnyAsync(feature => feature.Name.ToLower().Equals(name.ToLower()));
            return exists;
        }

        public async Task<bool> ExistsActiveFeatureByClientSystemIdAsync(int clientSystemId)
        {
            _logger.LogDebug("Validando existencia de Features ativas para algum cliente no banco de dados");
            return await _context.FeatureEntity
                .AsNoTracking()
                .AnyAsync(feat => feat.ClientSystemId == clientSystemId && feat.IsActive);
        }
    }
}
