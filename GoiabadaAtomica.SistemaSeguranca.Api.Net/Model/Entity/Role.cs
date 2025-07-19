namespace GoiabadaAtomica.ApiAutenticacao.Net.Model.entity
{
    public class Role
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required bool Status { get; set; }
        public ICollection<UserRole>? UserRole { get; set; }
    }
}
