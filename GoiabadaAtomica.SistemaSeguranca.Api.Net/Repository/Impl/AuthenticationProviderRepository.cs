using GoiabadaAtomica.ApiAutenticacao.Net.Data;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Impl
{
    public class AuthenticationProviderRepository : IAuthenticationProviderRepository
    {
        private readonly ILogger<IAuthenticationProviderRepository> _logger;
        private ApplicationContext _context;

        public AuthenticationProviderRepository(ILogger<IAuthenticationProviderRepository> logger, ApplicationContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<bool> ExistsActiveAuthenticationProviderByClientSystemIdAsync(int clientSystemId)
        {
            return await _context.AuthenticationProvider
                .AsNoTracking()
                .AnyAsync(ap => ap.ClientSystemId == clientSystemId);

        }
    }
}
