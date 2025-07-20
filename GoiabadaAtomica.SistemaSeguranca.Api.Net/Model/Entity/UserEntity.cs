namespace GoiabadaAtomica.ApiAutenticacao.Net.Model.entity
{
    public class UserEntity
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string FullName { get; set; }
        public required string PasswordHash { get; set; }
        public required string Email { get; set; }
        public required bool IsActive { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }
        public ICollection<UserRoleEntity> UserRole { get; set; }
    }
}
