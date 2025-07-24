namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Repository.Interface
{
    public interface IFeatureRepository
    {
        Task<bool> ExistsActiveFeatureByClientSystemIdAsync(int clientSystemId);
    }
}
