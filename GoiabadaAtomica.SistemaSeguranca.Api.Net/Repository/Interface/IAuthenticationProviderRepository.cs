namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Interface
{
    public interface IAuthenticationProviderRepository
    {
        Task<bool> HasActiveAuthenticationProviderByClientSystemIdAsync(int clientSystemId);
    }
}
