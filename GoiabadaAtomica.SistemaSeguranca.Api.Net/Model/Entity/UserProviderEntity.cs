using GoiabadaAtomica.ApiAutenticacao.Net.Model.entity;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.Entity
{
    public class UserProviderEntity
    {
        public int UserId { get; set; }
        public int AuthenticationProviderId { get; set; }
        public required string ExternalIdentifier { get; set; }
        public DateTime LinkedAt { get; set; } = DateTime.UtcNow;
        public UserEntity User { get; set; }
        public AuthenticationProviderEntity AuthenticationProvider { get; set; }
    }
}
