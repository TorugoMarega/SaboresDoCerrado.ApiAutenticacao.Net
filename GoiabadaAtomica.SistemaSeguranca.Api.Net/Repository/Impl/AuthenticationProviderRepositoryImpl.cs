using GoiabadaAtomica.ApiAutenticacao.Net.Data;
using GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Impl
{
    public class AuthenticationProviderRepositoryImpl : IAuthenticationProviderRepository
    {
        private readonly ILogger<AuthenticationProviderRepositoryImpl> _logger;
        private ApplicationContext _context;

        public AuthenticationProviderRepositoryImpl(ILogger<AuthenticationProviderRepositoryImpl> logger, ApplicationContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<bool> HasActiveAuthenticationProviderByClientSystemIdAsync(int clientSystemId)
        {
            return await _context.AuthenticationProvider
                .AsNoTracking()
                .AnyAsync(ap => ap.ClientSystemId == clientSystemId);
        }
    }
}
