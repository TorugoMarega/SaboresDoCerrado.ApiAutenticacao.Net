namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response.Tenant
{
    public class TenantDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
