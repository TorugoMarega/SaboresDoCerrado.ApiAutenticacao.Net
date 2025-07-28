namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response
{
    public class FeatureDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? ClientSystemId { get; set; }
        public bool IsActive { get; set; }
    }
}
