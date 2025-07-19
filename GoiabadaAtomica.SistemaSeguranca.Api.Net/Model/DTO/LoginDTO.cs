namespace GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO
{
    public class LoginDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; }
    }
}
