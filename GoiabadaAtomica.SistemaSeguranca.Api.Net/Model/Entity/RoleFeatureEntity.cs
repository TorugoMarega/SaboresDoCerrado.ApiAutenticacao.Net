using GoiabadaAtomica.ApiAutenticacao.Net.Model.entity;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.Entity
{
    public class RoleFeatureEntity
    {
        public int RoleId { get; set; }
        public int FeatureId { get; set; }
        public int ClientSystemId { get; set; }
        public RoleEntity Role { get; set; }
        public FeatureEntity Feature { get; set; }
        public ClientSystemEntity ClientSystem { get; set; }
    }
}
