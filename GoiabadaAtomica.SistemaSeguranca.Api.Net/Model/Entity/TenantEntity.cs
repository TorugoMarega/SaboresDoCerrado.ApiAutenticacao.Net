using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.Entity
{
    [Table("tbl_tenants")]
    public class TenantEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(150)]
        public string? ContactEmail { get; set; }

        [Required]
        [MaxLength(20)]
        public string? ContactPhone { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
