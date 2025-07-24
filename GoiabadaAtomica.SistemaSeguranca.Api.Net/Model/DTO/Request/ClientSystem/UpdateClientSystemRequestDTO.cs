namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Request.ClientSystem
{
    public class UpdateClientSystemRequestDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
