using GoiabadaAtomica.ApiAutenticacao.Net.Data;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Impl
{
    public class FeatureRepositoryImpl:IFeatureRepository
    {
        private readonly ApplicationContext _context;
        private readonly ILogger<IFeatureRepository> _logger;

        public FeatureRepositoryImpl(ApplicationContext context, ILogger<IFeatureRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> ExistsActiveFeatureByClientSystemIdAsync(int clientSystemId)
        {
            return await _context.FeatureEntity
                .AsNoTracking()
                .AnyAsync(feat => feat.ClientSystemId == clientSystemId && feat.IsActive);
        }
    }
}
