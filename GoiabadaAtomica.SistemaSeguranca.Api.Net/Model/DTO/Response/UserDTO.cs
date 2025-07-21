namespace GoiabadaAtomica.SistemaSeguranca.Api.Net.Model.DTO.Response
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public List<string> Roles { get; set; }
    }
}
