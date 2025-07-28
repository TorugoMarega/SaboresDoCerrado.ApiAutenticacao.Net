namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.Entity
{
    public class AuthenticationProviderEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Type { get; set; }
        public int? ConfigurationJson { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int? ClientSystemId { get; set; }
        public ClientSystemEntity? ClientSystem { get; set; }
    }
}
