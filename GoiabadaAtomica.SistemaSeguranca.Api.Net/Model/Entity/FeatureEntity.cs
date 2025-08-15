namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.Entity
{
    public class FeatureEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public bool IsActive { get; set; }
        public int? ClientSystemId { get; set; }
        public ClientSystemEntity ClientSystem { get; set; }
        public ICollection<RoleFeatureEntity> RoleFeature { get; set; }
    }
}
