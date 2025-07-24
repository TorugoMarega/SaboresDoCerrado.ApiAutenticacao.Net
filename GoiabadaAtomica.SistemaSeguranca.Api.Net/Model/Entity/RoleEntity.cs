namespace GoiabadaAtomica.ApiAutenticacao.Net.Model.entity
{
    public class RoleEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required bool IsActive { get; set; }
        public ICollection<UserRoleEntity>? UserRole { get; set; }
    }
}
