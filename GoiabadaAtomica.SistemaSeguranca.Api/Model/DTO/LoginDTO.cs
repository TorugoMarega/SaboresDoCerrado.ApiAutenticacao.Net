namespace GoiabadaAtomica.ApiAutenticacao.Net.Model.DTO
{
    public class LoginDTO
    {
        public int Id { get; set; }
        public string NomeUsuario { get; set; }
        public string Email { get; set; }
        public string HashSenha { get; set; }
        public bool IsAtivo { get; set; }
        public List<string> Perfis { get; set; }
    }
}
