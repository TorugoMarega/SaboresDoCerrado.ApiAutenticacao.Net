using GoiabadaAtomica.ApiAutenticacao.Net.Model.entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.Entity
{
    public class ClientSystemEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; }
        public required string ClientId { get; set; }
        public required string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        [Required]
        public int TenantId { get; set; }

        [ForeignKey("TenantId")]
        public TenantEntity Tenant { get; set; }
        public ICollection<RoleEntity> Roles { get; set; } = new List<RoleEntity>();
        public ICollection<FeatureEntity> Features { get; set; } = new List<FeatureEntity>();
    }
}
