namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response.ClientSystem
{
    public class ClientSystemDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ClientId { get; set; }
        public bool Status { get; set; } = true;
    }
}
